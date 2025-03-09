using iPhoneBE.Data.Entities;
using iPhoneBE.Data.Helper;
using iPhoneBE.Data.Interfaces;
using iPhoneBE.Data.Model;
using iPhoneBE.Data.Models.AdminModel;
using iPhoneBE.Data.Models.OrderModel;
using iPhoneBE.Service.Extentions;
using iPhoneBE.Service.Interfaces;
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

        public OrderServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
    }
}
