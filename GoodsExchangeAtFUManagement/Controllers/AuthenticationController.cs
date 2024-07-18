using GoodsExchangeAtFUManagement.Middlewares;
using GoodsExchangeAtFUManagement.Repository.DTOs.UserDTOs;
using GoodsExchangeAtFUManagement.Service.Services.AuthenticationService;
using GoodsExchangeAtFUManagement.Service.Services.UserServices;
using GoodsExchangeAtFUManagement.Service.Ultis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoodsExchangeAtFUManagement.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthenticationController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IUserService userService, IAuthenticationService authenticationService)
        {
            _userService = userService;
            _authenticationService = authenticationService;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(UserRegisterRequestModel request)
        {
            await _authenticationService.Register(request);
            return Ok("Register successfully!");
        }

        [HttpPost]
        [Route("register-test")]
        public async Task<IActionResult> RegisterForTest(UserRegisterRequestTestingModel request)
        {
            await _userService.Register(request);
            return Ok("Register successfully!");
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Authenticate(UserLoginRequestModel request)
        {
            var user = await _authenticationService.Authenticate(request);
            return Ok(user);
        }

        [HttpPost]
        [Route("password/reset")]
        public async Task<IActionResult> ResetPassword(UserResetPasswordRequestModel request)
        {
            await _userService.ResetPassword(request);
            return Ok();
        }
    }
}
