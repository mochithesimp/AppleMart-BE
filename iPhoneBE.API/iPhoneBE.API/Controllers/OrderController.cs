using AutoMapper;
using iPhoneBE.Data.Models.OrderModel;
using iPhoneBE.Data.ViewModels.OrderDTO;
using iPhoneBE.Service.Interfaces;
using iPhoneBE.Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
