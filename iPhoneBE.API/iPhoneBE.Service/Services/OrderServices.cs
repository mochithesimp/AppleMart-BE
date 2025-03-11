﻿using iPhoneBE.Data.Entities;
using iPhoneBE.Data.Helper;
using iPhoneBE.Data.Interfaces;
using iPhoneBE.Data.Model;
using iPhoneBE.Data.Models.AdminModel;
using iPhoneBE.Data.Models.OrderModel;
using iPhoneBE.Service.Extentions;
using iPhoneBE.Service.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iPhoneBE.Service.Services
{
    public class OrderServices : IOrderServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;

        public OrderServices(IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<object> GetOrdersAsync(Guid? userId, string? status, TimeModel model, string userRole, Guid currentUserId)
        {
            var query = _unitOfWork.OrderRepository.GetAllQueryable();

            // Nếu user là Customer, chỉ cho phép lấy đơn hàng của họ
            if (userRole == "Customer")
            {
                query = query.Where(o => o.UserID == currentUserId.ToString());
            }
            else if (userId.HasValue)
            {
                // Nếu là Admin hoặc role cao hơn, có thể lọc theo userId (nếu có)
                query = query.Where(o => o.UserID == userId.Value.ToString());
            }

            var orders = await query
                .FilterByStatus(status)
                .FilterByYear(model.Year)
                .FilterByQuarter(model.Quarter, model.Year)
                .FilterByMonth(model.Month, model.Year)
                .FilterByDay(model.Day, model.Month, model.Year)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return new
            {
                IsFiltered = model.Year.HasValue || model.Quarter.HasValue || model.Month.HasValue || model.Day.HasValue || !string.IsNullOrEmpty(status) || userId.HasValue,
                Year = model.Year,
                Quarter = model.Quarter,
                Month = model.Month,
                Day = model.Day,
                Status = status,
                UserId = userRole == "Customer" ? currentUserId : userId, // Nếu là Customer thì buộc phải lấy của họ
                Orders = orders
            };
        }


        public async Task<Order> GetByIdAsync(int id)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(id, o => o.OrderDetails);
            if (order == null)
                throw new KeyNotFoundException($"Order with ID {id} not found.");

            return order;
        }

        public async Task<Order> AddAsync(OrderModel model)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var order = new Order
                {
                    UserID = model.UserID,
                    ShipperID = model.ShipperID,
                    OrderDate = model.OrderDate,
                    Address = model.Address,
                    PaymentMethod = model.PaymentMethod,
                    ShippingMethodID = model.ShippingMethodID,
                    OrderStatus = OrderStatusHelper.Pending,
                    VoucherID = (model.VoucherID == 0 || model.VoucherID == null) ? null : model.VoucherID,
                    Total = model.Total,
                    IsDeleted = false
                };

                var result = await _unitOfWork.OrderRepository.AddAsync(order);
                await _unitOfWork.SaveChangesAsync();

                var productIds = model.OrderDetails.Select(od => od.ProductItemID).ToList();
                var products = await _unitOfWork.ProductItemRepository.GetAllAsync(p => productIds.Contains(p.ProductID));

                foreach (var item in model.OrderDetails)
                {
                    var product = products.FirstOrDefault(p => p.ProductID == item.ProductItemID);

                    if (product == null)
                    {
                        throw new KeyNotFoundException("product not found");
                    }

                    if (product.Quantity < item.Quantity)
                    {
                        throw new InvalidOperationException($"Insufficient stock for Product {item.ProductItemID}. Available: {product.Quantity}, Requested: {item.Quantity}");
                    }

                    var orderDetail = new OrderDetail
                    {
                        OrderID = result.OrderID,
                        ProductItemID = item.ProductItemID,
                        Price = item.Price,
                        Quantity = item.Quantity,
                        IsDeleted = false
                    };

                    await _unitOfWork.OrderDetailRepository.AddAsync(orderDetail);

                    product.Quantity -= item.Quantity;
                    await _unitOfWork.ProductItemRepository.Update(product);
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return result;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                Console.WriteLine($"Error in AddAsync: {ex.Message} - {ex.InnerException}");
                throw;
            }
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus, User user, string? shipperId = null)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new KeyNotFoundException("Order not found.");
            }

            if (order.OrderStatus == newStatus)
            {
                throw new InvalidOperationException($"The order is already in '{newStatus}' status.");
            }

            bool isCustomer = await _userManager.IsInRoleAsync(user, "Customer");
            bool isStaff = await _userManager.IsInRoleAsync(user, "Staff");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                if (isCustomer)
                {
                    await HandleCustomerStatusChange(order, newStatus);
                }
                else if (isStaff)
                {
                    await HandleStaffStatusChange(order, newStatus, shipperId);
                }
                else
                {
                    throw new UnauthorizedAccessException("Unauthorized user.");
                }

                await _unitOfWork.OrderRepository.Update(order);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                Console.WriteLine($"Error in UpdateOrderStatusAsync: {ex.Message}");
                throw;
            }
        }



        private async Task HandleCustomerStatusChange(Order order, string newStatus)
        {
            if (newStatus == OrderStatusHelper.Cancelled && order.OrderStatus == OrderStatusHelper.Pending)
            {
                order.OrderStatus = OrderStatusHelper.Cancelled;
                await RestoreProductStock(order.OrderID);
            }
            else if (newStatus == OrderStatusHelper.RefundRequested &&
                     (order.OrderStatus == OrderStatusHelper.Delivered || order.OrderStatus == OrderStatusHelper.Completed))
            {
                order.OrderStatus = OrderStatusHelper.RefundRequested;
            }
            else
            {
                throw new InvalidOperationException("Customers can only cancel pending orders or request refunds for delivered/completed orders.");
            }
        }


        private async Task HandleStaffStatusChange(Order order, string newStatus, string? shipperId)
        {
            var validTransitions = new Dictionary<string, List<string>>
            {
                { OrderStatusHelper.Pending, new List<string> { OrderStatusHelper.Processing } },
                { OrderStatusHelper.Paid, new List<string> { OrderStatusHelper.Processing } },
                { OrderStatusHelper.Processing, new List<string> { OrderStatusHelper.Shipped } },
                { OrderStatusHelper.Shipped, new List<string> { OrderStatusHelper.Delivered } },
                { OrderStatusHelper.Delivered, new List<string> { OrderStatusHelper.Completed, OrderStatusHelper.RefundRequested } },
                { OrderStatusHelper.RefundRequested, new List<string> { OrderStatusHelper.Refunded } }
            };

            if (!validTransitions.ContainsKey(order.OrderStatus) || !validTransitions[order.OrderStatus].Contains(newStatus))
            {
                throw new InvalidOperationException($"Invalid status transition from '{order.OrderStatus}' to '{newStatus}'.");
            }

            if (order.OrderStatus == OrderStatusHelper.Processing && newStatus == OrderStatusHelper.Shipped)
            {
                if (string.IsNullOrEmpty(shipperId))
                {
                    throw new ArgumentException("A shipper must be assigned before shipping.");
                }

                if (IsShipperBusy(shipperId))
                {
                    throw new InvalidOperationException("The assigned shipper is already delivering another order.");
                }

                order.ShipperID = shipperId;
            }
            else if (order.OrderStatus == OrderStatusHelper.RefundRequested && newStatus == OrderStatusHelper.Refunded)
            {
                if (order.PaymentMethod == "PayPal")
                {
                    //todo
                    bool refundSuccess = await ProcessPayPalRefund(order);
                    if (!refundSuccess)
                    {
                        throw new InvalidOperationException("PayPal refund failed.");
                    }
                }

                await RestoreProductStock(order.OrderID);
            }

            order.OrderStatus = newStatus;
        }

        private bool IsShipperBusy(string shipperId)
        {
            return _unitOfWork.OrderRepository.GetAllQueryable().Any(o => o.ShipperID == shipperId && o.OrderStatus == OrderStatusHelper.Shipped);
        }

        private async Task RestoreProductStock(int orderId)
        {
            var orderDetails = await _unitOfWork.OrderDetailRepository
                 .GetAllQueryable()
                 .Where(od => od.OrderID == orderId)
                 .AsNoTracking()
                 .ToListAsync();

            var productIds = orderDetails.Select(od => od.ProductItemID).Distinct().ToList();

            var products = await _unitOfWork.ProductItemRepository
                .GetAllQueryable()
                .Where(p => productIds.Contains(p.ProductItemID))
                .AsNoTracking()
                .ToListAsync();


            foreach (var item in orderDetails)
            {
                var product = products.FirstOrDefault(p => p.ProductItemID == item.ProductItemID);
                if (product != null)
                {
                    product.Quantity += item.Quantity;
                    await _unitOfWork.ProductItemRepository.Update(product);
                }
            }

            await _unitOfWork.SaveChangesAsync();

        }

        //todo
        private async Task<bool> ProcessPayPalRefund(Order order)
        {
            try
            {
                //var paypalService = new PayPalService(); // Giả sử có class tích hợp PayPal
                //bool refundSuccess = await paypalService.RefundPayment(order.PaymentTransactionId, order.Total);

                //return refundSuccess;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PayPal refund error: {ex.Message}");
                return false;
            }
        }


    }
}
