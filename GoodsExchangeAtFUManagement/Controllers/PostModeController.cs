using BusinessObjects.DTOs.PostModeDTOs;
using GoodsExchangeAtFUManagement.Service.Services.PostModeServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GoodsExchangeAtFUManagement.Controllers
{
    [Route("api/post-mode")]
    [ApiController]
    [Authorize]
    public class PostModeController : ControllerBase
    {
        private readonly IPostModeService _postModeService;

        public PostModeController(IPostModeService postModeService)
        {
            _postModeService = postModeService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("create")]
        public async Task<IActionResult> CreatePostMode(PostModeCreateRequestModel requestModel)
        {
            await _postModeService.CreatePostMode(requestModel);
            return Ok("Create post mode successfully");
        }

        [HttpGet]
        [Route("view/active")]
        public async Task<IActionResult> ViewActivePostModes()
        {
            var list = await _postModeService.ViewActivePostMode();
            return Ok(list);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("view/all")]
        public async Task<IActionResult> ViewAllPostModes()
        {
            var list = await _postModeService.ViewAllPostMode();
            return Ok(list);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("view/{id}")]
        public async Task<IActionResult> ViewPostModeById(string id)
        {
            var postMode = await _postModeService.GetPostModeById(id);
            return Ok(postMode);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        [Route("update/{id}")]
        public async Task<IActionResult> UpdatePostMode(PostModeUpdateRequestModel requestModel, string id)
        {
            await _postModeService.UpdatePostMode(requestModel, id);
            return Ok("Post mode update successfully");
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        [Route("soft-remove")]
        public async Task<IActionResult> SoftRemovePostModes(List<string> ids)
        {
            await _postModeService.SoftRemoveList(ids);
            return Ok("Soft remove successfully");
        }
    }
}
