using AutoMapper;
using iPhoneBE.Data.Models.AdminModel;
using iPhoneBE.Data.Models.OrderModel;
using iPhoneBE.Data.ViewModels.OrderVM;
using iPhoneBE.Service.Interfaces;
using iPhoneBE.Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace iPhoneBE.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderServices _orderServices;
        private readonly IMapper _mapper;

        public OrderController(IOrderServices orderServices, IMapper mapper)
        {
            _orderServices = orderServices;
            _mapper = mapper;
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

            return CreatedAtAction(nameof(GetOrderById), new { id = orderViewModel.OrderID }, orderViewModel);
        }

    }
}
