using iPhoneBE.Data.Models.AdminModel;
using iPhoneBE.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace iPhoneBE.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminServices _adminServices;

        public AdminController(IAdminServices adminServices)
        {
            _adminServices = adminServices;
        }

        [HttpGet("get-total-user")]
        public async Task<int> GetTotalUserAsync()
        {
            var result = await _adminServices.GetTotalUserAsync();

            return result;
        }

        [HttpGet("get-total-revenue")]
        public async Task<IActionResult> GetTotalRevenueAsync([FromQuery] TimeModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _adminServices.GetTotalRevenueAsync(model);
            return Ok(result);
        }

        [HttpGet("get-top-selling-product-items")]
        public async Task<IActionResult> GetTopSellingProductItemsAsync([FromQuery] TimeModel model, int? topN)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _adminServices.GetTopSellingProductItemsAsync(model, topN);
            return Ok(result);
        }

        [HttpGet("get-top-costumers")]
        public async Task<IActionResult> GetTopCustomersAsync([FromQuery] TimeModel model, int? topN)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _adminServices.GetTopCustomersAsync(model, topN);
            return Ok(result);
        }
    }
}
