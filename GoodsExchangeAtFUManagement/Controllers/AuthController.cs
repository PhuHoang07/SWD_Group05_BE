using GoodsExchangeAtFUManagement.Middlewares;
using GoodsExchangeAtFUManagement.Repository.DTOs.UserDTOs;
using GoodsExchangeAtFUManagement.Service.Services.UserServices;
using GoodsExchangeAtFUManagement.Service.Ultis;
using Microsoft.AspNetCore.Mvc;

namespace GoodsExchangeAtFUManagement.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IUserService _userService;
        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(UserRegisterRequestModel request)
        {
            await _userService.RegisterAccount(request);
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
        public async Task<IActionResult> Login(UserLoginRequestModel request)
        {
            var user = await _userService.Login(request);
            return Ok(user);
        }
    }
}
