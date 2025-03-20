using AutoMapper;
using iPhoneBE.Data.Helper;
using iPhoneBE.Data.Model;
using iPhoneBE.Data.Models.OrderModel;
using iPhoneBE.Data.Models.UserModel;
using iPhoneBE.Data.ViewModels.UserVM;
using iPhoneBE.Service.Interfaces;
using iPhoneBE.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace iPhoneBE.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;
        private readonly IOrderServices _orderServices;
        private readonly IMapper _mapper;

        public UserController(IUserServices userServices, IOrderServices orderServices, IMapper mapper)
        {
            _userServices = userServices;
            _orderServices = orderServices;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        //[Authorize(Roles = $"{RolesHelper.Staff}, {RolesHelper.Admin}")]
        public async Task<IActionResult> GetAll([FromQuery] string? role)
        {
            var result = await _userServices.GetAllAsync(role);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var (user, role) = await _userServices.GetUserWithRoleAsync(id);

            if (user == null)
            {
                return NotFound(new { message = $"User with ID {id} not found." });
            }

            var userViewModel = _mapper.Map<UserViewModel>(user);
            userViewModel.Role = role;

            return Ok(userViewModel);
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UserModel updateCategory)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userServices.UpdateAsync(id, updateCategory);
            return Ok(user);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var deletedUser = await _userServices.DeleteAsync(id);
            return Ok(deletedUser);
        }

        [HttpPut("{orderId}/status")]
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
            if (role != null && role != RolesHelper.Customer)
            {
                return Unauthorized(new { message = "User not customer" });
            }

            // Validate status for customer operations
            var validCustomerStatuses = new List<string>
                {
                    OrderStatusHelper.Cancelled,
                    OrderStatusHelper.RefundRequested,
                    OrderStatusHelper.Completed
                };

            if (!validCustomerStatuses.Contains(model.NewStatus))
            {
                return BadRequest(new { message = $"Invalid status for customer. Allowed values: {string.Join(", ", validCustomerStatuses)}" });
            }
                bool result = await _orderServices.UpdateOrderStatusAsync(orderId, model.NewStatus, user);
                return result
                    ? Ok(new { message = "Order status updated successfully." })
                    : BadRequest(new { message = "Failed to update order status." });
        }
    }
}