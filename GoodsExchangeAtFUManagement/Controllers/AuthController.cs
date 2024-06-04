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
            try
            {
                await _userService.RegisterAccount(request);
                return Ok("Register successfully!");
            }
            catch (CustomException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(UserLoginRequestModel request)
        {
            try
            {
                var user = await _userService.Login(request);
                return Ok(user);
            }
            catch (CustomException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
