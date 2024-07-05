
using BusinessObjects.DTOs.UserDTOs;
using GoodsExchangeAtFUManagement.Service.Services.UserServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GoodsExchangeAtFUManagement.Controllers
{
    [Route("api/user")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }      

        [HttpGet]
        [Route("view-all")]
        public async Task<IActionResult> GetAllUser(int pageIndex, int pageSize, string searchQuery = null)
        {
            
            var users = await _userService.GetAllUser(searchQuery, pageIndex, pageSize);
            return Ok(users);
        }

        [HttpGet]
        [Route("view/{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userService.GetUserById(id);
            return Ok(user);
        }

        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequestModel request)
        {

            await _userService.UpdateUser(request);
            return Ok("User updated successfully!");
        }

        [HttpPut]
        [Route("soft-remove")]
        public async Task<IActionResult> SoftRemoveUser(string id)
        {
            await _userService.DeleteUser(id);
            return Ok("User deleted successfully!");
        }
    }
}
