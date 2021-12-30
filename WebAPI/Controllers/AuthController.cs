using Business.Abstract;
using Entities.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public ActionResult Register(UserForRegisterDto userForRegisterDto)
        {
            var userCheck = _authService.UserExists(userForRegisterDto.Email);
            if (!userCheck.Success)
            {
                return BadRequest(userCheck.Message);
            }

            var registerResult = _authService.Register(userForRegisterDto);
            var tokenResult = _authService.CreateAccessToken(registerResult.Data);
            if (!tokenResult.Success)
            {
                return BadRequest(tokenResult.Message);
            }

            return Ok(tokenResult);
        }

        [HttpPost("login")]
        public ActionResult Login(UserForLoginDto userForLoginDto)
        {
            var userToLogin = _authService.Login(userForLoginDto);
            if (!userToLogin.Success)
            {
                return BadRequest(userToLogin.Message);
            }

            var tokenResult = _authService.CreateAccessToken(userToLogin.Data);
            if (!tokenResult.Success)
            {
                return BadRequest(tokenResult.Message);
            }

            return Ok(tokenResult);
        }
    }
}
