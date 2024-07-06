using BusinessObjects.DTOs.ProductPostDTOs;
using GoodsExchangeAtFUManagement.Service.Services.ProductPostServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GoodsExchangeAtFUManagement.Controllers
{
    [Route("api/product-post")]
    [ApiController]
    public class ProductPostController : ControllerBase
    {
        private readonly IProductPostService _productPostService;
        public ProductPostController(IProductPostService productPostService)
        {
            _productPostService = productPostService;
        }

        [HttpGet]
        [Route("all")]
        [Authorize(Roles = "Admin, Moderator")]
        public async Task<IActionResult> GetAllProductPost(int? pageIndex, [FromQuery] PostSearchModel searchModel, string status)
        {
            var result = await _productPostService.ViewAllPostWithStatus(pageIndex, searchModel, status);
            return Ok(result);
        }

        [HttpGet]
        [Route("me")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetOwnProductPost(int? pageIndex, [FromQuery] PostSearchModel searchModel, string status)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var result = await _productPostService.ViewOwnPostWithStatus(pageIndex, searchModel, status, token);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CreateProductPost(ProductPostCreateRequestModel requestModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            await _productPostService.CreateWaitingProductPost(requestModel, token);
            return Ok("Create post successfully. Please wait for moderator approving your post");
        }

        [HttpPut]
        [Route("approve/{id}")]
        [Authorize(Roles = "Moderator")]
        public async Task<IActionResult> ApproveProductPost([FromBody] string status, string id)
        {
            await _productPostService.ApprovePost(status, id);
            return Ok("Approve post successfully");
        }
    }
}
