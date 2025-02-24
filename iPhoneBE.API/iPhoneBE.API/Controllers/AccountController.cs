using iPhoneBE.Data.Models.AuthenticationModel;
using iPhoneBE.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Net;

namespace iPhoneBE.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountServices _accountServices;
        //private readonly IEmailHelper _emailHelper;

        public AccountController(IAccountServices accountServices/*, IEmailHelper emailHelper*/)
        {
            _accountServices = accountServices;
            //_emailHelper = emailHelper;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _accountServices.LoginAsync(model);

            if (result == null)
            {
                return Unauthorized();
            }

            return Ok(result);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(CancellationToken cancellationToken, RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _accountServices.RegisterAsync(model);
            if (result.Succeeded)
            {
                //await _emailHelper.SendMailAsync(cancellationToken, new EmailRequestModel
                //{
                //    To = result.
                //})

                return Ok(result);
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenModel refreshTokenModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (refreshTokenModel == null) return BadRequest("Could not get refresh token");

                var result = await _accountServices.ValidateRefreshToken(refreshTokenModel);
                if (result == null)
                {
                    return Unauthorized("Invalid email or password");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }
    }
}
