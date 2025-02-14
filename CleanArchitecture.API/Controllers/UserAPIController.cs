﻿using CleanArchitecture.ApplicationCore.Commons;
using CleanArchitecture.ApplicationCore.Entities.DTOs;
using CleanArchitecture.ApplicationCore.Interfaces.Commons;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAPIController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserAPIController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("GetAllUser")]
        public async Task<ActionResult<ResponseDTO>> GetAllUser()
        {
            return Ok(await _userService.GetAllUserAsync());
        }

        [HttpPut("LockUnlockUser")]
        public async Task<ActionResult<ResponseDTO>> LockUnlockUser(string userId)
        {
            return Ok(await _userService.LockUnlockAsync(userId));
        }

        [HttpGet("GetUserInfo")]
        public async Task<ActionResult<ResponseDTO>> GetUserInfo(string userId)
        {
            return Ok(await _userService.GetUserInfoAsync(userId));
        }

        [HttpPut("UpdateUser")]
        public async Task<ActionResult<ResponseDTO>> UpdateUser([FromBody] AppUserDTO userDTO)
        {
            return Ok(await _userService.UpdateUserAsync(userDTO));
        }

        [HttpPut("ChangePassword")]
        public async Task<ActionResult<ResponseDTO>> ChangePassword([FromBody] ChangePasswordDTO passwordDTO)
        {
            return Ok(await _userService.ChangePasswordAsync(passwordDTO));
        }

        [HttpPost("ForgotPassword")]
        public async Task<ActionResult<ResponseDTO>> ForgotPassword([FromBody] string email)
        {
            return Ok(await _userService.ForgotPasswordAsync(email));
        }

        [HttpPost("ResetPassword")]
        public async Task<ActionResult<ResponseDTO>> ResetPassword([FromBody] ResetPasswordDTO passwordDTO)
        {
            return Ok(await _userService.ResetPasswordAsync(passwordDTO));
        }
    }
}
