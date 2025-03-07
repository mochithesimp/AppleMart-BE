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

    }
}
