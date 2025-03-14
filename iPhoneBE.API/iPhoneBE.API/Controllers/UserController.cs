﻿using AutoMapper;
using iPhoneBE.Data.Models.UserModel;
using iPhoneBE.Data.ViewModels.UserVM;
using iPhoneBE.Service.Interfaces;
using iPhoneBE.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iPhoneBE.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;
        private readonly IMapper _mapper;

        public UserController(IUserServices userServices, IMapper mapper)
        {
            _userServices = userServices;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        //[Authorize(Roles = $"{RolesHelper.Staff}, {RolesHelper.Admin}")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _userServices.GetAllAsync();
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

    }
}