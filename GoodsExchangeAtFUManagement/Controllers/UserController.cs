
using BusinessObjects.DTOs.UserDTOs;
using GoodsExchangeAtFUManagement.Service.Services.UserServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GoodsExchangeAtFUManagement.Controllers
{
    [Route("api")]
    [ApiController]
    
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }      

        [HttpGet]
        [Route("admin/view-all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUser(int pageIndex, int pageSize, string searchQuery = null)
        {
            
            var users = await _userService.GetAllUser(searchQuery, pageIndex, pageSize);
            return Ok(users);
        }

        [HttpGet]
        [Route("user/view/{id}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userService.GetUserById(id);
            return Ok(user);
        }

        [HttpPut]
        [Route("user/update")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequestModel request, string id)
        {

            await _userService.UpdateUser(request, id);
            return Ok("User updated successfully!");
        }

        [HttpPut]
        [Route("admin/update")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUserForAdmin([FromBody] AdminUpdateUserResponseModel request, string id)
        {

            await _userService.UpdateUserForAdmin(request, id);
            return Ok("User updated successfully!");
        }

        [HttpPut]
        [Route("admin/soft-remove")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SoftRemoveUser(string id)
        {
            await _userService.DeleteUser(id);
            return Ok("User deleted successfully!");
        }
    }
}
