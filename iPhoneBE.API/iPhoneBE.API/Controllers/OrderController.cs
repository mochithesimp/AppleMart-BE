using AutoMapper;
using iPhoneBE.Data.Helper;
using iPhoneBE.Data.Models.AdminModel;
using iPhoneBE.Data.Models.OrderModel;
using iPhoneBE.Data.ViewModels.OrderVM;
using iPhoneBE.Service.Interfaces;
using iPhoneBE.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using iPhoneBE.API.Hubs;
using Microsoft.AspNetCore.Identity;
using iPhoneBE.Data.Entities;
using iPhoneBE.Data.Model;

namespace iPhoneBE.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderServices _orderServices;
        private readonly IUserServices _userServices;
        private readonly IMapper _mapper;
        private readonly IHubContext<NotificationHub> _notificationHub;
        private readonly INotificationServices _notificationServices;
        private readonly UserManager<User> _userManager;

        public OrderController(
            IOrderServices orderServices,
            IUserServices userServices,
            IMapper mapper,
            IHubContext<NotificationHub> notificationHub,
            INotificationServices notificationServices,
            UserManager<User> userManager)
        {
            _orderServices = orderServices;
            _userServices = userServices;
            _mapper = mapper;
            _notificationHub = notificationHub;
            _notificationServices = notificationServices;
            _userManager = userManager;
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders([FromQuery] Guid? userId, [FromQuery] string? status, [FromQuery] TimeModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUserId = Guid.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
            throw new UnauthorizedAccessException("UserId is missing in the token.")
            );
            var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ??
                           throw new UnauthorizedAccessException("User role is missing in the token.");

            var result = await _orderServices.GetOrdersAsync(userId, status, model, userRole, currentUserId);
            return Ok(result);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderServices.GetByIdAsync(id);
            var orderViewModel = _mapper.Map<OrderViewModel>(order);
            return Ok(orderViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddOrder([FromBody] OrderModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var order = await _orderServices.AddAsync(model);
            var orderViewModel = _mapper.Map<OrderViewModel>(order);

            // Send notification to admin and staff users from the server side
            try
            {
                var customerName = "Unknown";

                if (!string.IsNullOrEmpty(model.UserID))
                {
                    var user = await _userManager.FindByIdAsync(model.UserID);
                    if (user != null)
                    {
                        customerName = user.UserName ?? user.Email ?? "Customer";
                    }
                }

                // Find all admins and staff
                var adminUsers = await _userManager.GetUsersInRoleAsync(RolesHelper.Admin);
                var staffUsers = await _userManager.GetUsersInRoleAsync(RolesHelper.Staff);

                foreach (var user in adminUsers.Union(staffUsers))
                {
                    var header = "New Order Placed";
                    var content = $"Customer {customerName} has placed a new order (ID: {order.OrderID})";

                    var notification = await _notificationServices.CreateNotification(user.Id, header, content);

                    await _notificationHub.Clients.Group($"User_{user.Id}").SendAsync(
                        "ReceiveNotification",
                        notification
                    );

                    var unreadCount = await _notificationServices.GetUnreadCount(user.Id);
                    await _notificationHub.Clients.Group($"User_{user.Id}").SendAsync(
                        "UpdateUnreadCount",
                        unreadCount
                    );
                }
            }
            catch (Exception ex)
            {
                // Log but don't fail the order if notification fails
                Console.WriteLine($"Error sending order notification: {ex.Message}");
            }

            return CreatedAtAction(nameof(GetOrderById), new { id = orderViewModel.OrderID }, orderViewModel);
        }

        [HttpPut("{orderId}/status")]
        [Authorize]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromQuery] UpdateOrderStatusModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var (user, role) = await _userServices.GetUserWithRoleAsync(userId);
            if (user == null)
            {
                return Unauthorized(new { message = "User not found" });
            }

            var validStatuses = new List<string>
                {
                    OrderStatusHelper.Pending, OrderStatusHelper.Paid, OrderStatusHelper.Processing,
                    OrderStatusHelper.Shipped, OrderStatusHelper.Delivered, OrderStatusHelper.Completed,
                    OrderStatusHelper.Cancelled, OrderStatusHelper.RefundRequested, OrderStatusHelper.Refunded
                };

            if (!validStatuses.Contains(model.NewStatus))
            {
                return BadRequest(new { message = $"Invalid status '{model.NewStatus}'. Allowed values: {string.Join(", ", validStatuses)}" });
            }

            bool result = await _orderServices.UpdateOrderStatusAsync(orderId, model.NewStatus, user, model.ShipperId);

            // Send notifications based on order status changes
            if (result)
            {
                try
                {
                    // Get the order to get user information
                    var order = await _orderServices.GetByIdAsync(orderId);
                    var customerUser = await _userManager.FindByIdAsync(order.UserID);
                    var customerName = customerUser?.UserName ?? customerUser?.Email ?? "Unknown Customer";

                    // Send appropriate notifications based on the new status
                    if (model.NewStatus == OrderStatusHelper.Cancelled)
                    {
                        // Already handled in the existing code
                        // Notify admins and staff about cancellation
                        var adminUsers = await _userManager.GetUsersInRoleAsync(RolesHelper.Admin);
                        var staffUsers = await _userManager.GetUsersInRoleAsync(RolesHelper.Staff);

                        foreach (var adminStaffUser in adminUsers.Union(staffUsers))
                        {
                            var header = "Order Cancelled";
                            var content = $"Customer {customerName} has cancelled order (ID: {orderId})";

                            var notification = await _notificationServices.CreateNotification(adminStaffUser.Id, header, content);

                            await _notificationHub.Clients.Group($"User_{adminStaffUser.Id}").SendAsync(
                                "ReceiveNotification",
                                notification
                            );

                            var unreadCount = await _notificationServices.GetUnreadCount(adminStaffUser.Id);
                            await _notificationHub.Clients.Group($"User_{adminStaffUser.Id}").SendAsync(
                                "UpdateUnreadCount",
                                unreadCount
                            );
                        }

                        // Also notify the customer
                        if (customerUser != null)
                        {
                            var header = "Order Cancelled";
                            var content = $"Dear customer, your Order (ID: {orderId}) has been cancelled by our moderators.";

                            var notification = await _notificationServices.CreateNotification(order.UserID, header, content);

                            await _notificationHub.Clients.Group($"User_{order.UserID}").SendAsync(
                                "ReceiveNotification",
                                notification
                            );

                            var unreadCount = await _notificationServices.GetUnreadCount(order.UserID);
                            await _notificationHub.Clients.Group($"User_{order.UserID}").SendAsync(
                                "UpdateUnreadCount",
                                unreadCount
                            );
                        }
                    }
                    else if (model.NewStatus == OrderStatusHelper.Processing)
                    {
                        // Notify the customer that their order has been confirmed
                        if (customerUser != null)
                        {
                            var header = "Order Confirmed";
                            var content = $"Dear customer, your Order (ID: {orderId}) has been confirmed. A shipper will be assigned soon to deliver to you.";

                            var notification = await _notificationServices.CreateNotification(order.UserID, header, content);

                            await _notificationHub.Clients.Group($"User_{order.UserID}").SendAsync(
                                "ReceiveNotification",
                                notification
                            );

                            var unreadCount = await _notificationServices.GetUnreadCount(order.UserID);
                            await _notificationHub.Clients.Group($"User_{order.UserID}").SendAsync(
                                "UpdateUnreadCount",
                                unreadCount
                            );
                        }
                    }
                    else if (model.NewStatus == OrderStatusHelper.Shipped)
                    {
                        // Notify the customer that their order has been shipped
                        if (customerUser != null)
                        {
                            var header = "Order Shipped";
                            var content = $"Dear customer, your Order (ID: {orderId}) has been shipped and is on its way to you.";

                            var notification = await _notificationServices.CreateNotification(order.UserID, header, content);

                            await _notificationHub.Clients.Group($"User_{order.UserID}").SendAsync(
                                "ReceiveNotification",
                                notification
                            );

                            var unreadCount = await _notificationServices.GetUnreadCount(order.UserID);
                            await _notificationHub.Clients.Group($"User_{order.UserID}").SendAsync(
                                "UpdateUnreadCount",
                                unreadCount
                            );
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log but don't fail the order update if notification fails
                    Console.WriteLine($"Error sending order status notification: {ex.Message}");
                }
            }

            return result
                ? Ok(new { message = "Order status updated successfully." })
                : BadRequest(new { message = "Failed to update order status." });
        }

    }
}
