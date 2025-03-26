using AutoMapper;
using iPhoneBE.Data.Entities;
using iPhoneBE.Data.Models.PaypalModel;
using iPhoneBE.Data.ViewModels.PaypalVM;
using iPhoneBE.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iPhoneBE.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaypalController : ControllerBase
    {
        private readonly IPaypalTransactionServices _paypalTransactionServices;
        private readonly IMapper _mapper;

        public PaypalController(IPaypalTransactionServices paypalTransactionServices, IMapper mapper)
        {
            _paypalTransactionServices = paypalTransactionServices;
            _mapper = mapper;
        }

        [HttpPost("create-transaction")]
        public async Task<ActionResult<PaypalTransactionViewModel>> CreateTransaction([FromBody] CreatePaypalTransactionModel model)
        {
            try
            {
                var transaction = await _paypalTransactionServices.CreateTransactionAsync(model);
                return Ok(_mapper.Map<PaypalTransactionViewModel>(transaction));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("transaction/{id}")]
        public async Task<ActionResult<PaypalTransactionViewModel>> GetTransactionById(int id)
        {
            try
            {
                var transaction = await _paypalTransactionServices.GetTransactionByIdAsync(id);
                return Ok(_mapper.Map<PaypalTransactionViewModel>(transaction));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("order/{orderId}/transactions")]
        public async Task<ActionResult<IEnumerable<PaypalTransactionViewModel>>> GetTransactionsByOrderId(int orderId)
        {
            try
            {
                var transactions = await _paypalTransactionServices.GetTransactionsByOrderIdAsync(orderId);
                return Ok(_mapper.Map<IEnumerable<PaypalTransactionViewModel>>(transactions));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("transaction/{id}/status")]
        public async Task<ActionResult<PaypalTransactionViewModel>> UpdateTransactionStatus(int id, [FromBody] string status)
        {
            try
            {
                var transaction = await _paypalTransactionServices.UpdateTransactionStatusAsync(id, status);
                return Ok(_mapper.Map<PaypalTransactionViewModel>(transaction));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("transaction/{id}/refund")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PaypalTransactionViewModel>> ProcessRefund(int id)
        {
            try
            {
                var transaction = await _paypalTransactionServices.ProcessRefundAsync(id);
                return Ok(_mapper.Map<PaypalTransactionViewModel>(transaction));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to process refund", details = ex.Message });
            }
        }
    }
}