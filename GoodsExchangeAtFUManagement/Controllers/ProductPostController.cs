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
        [Route("waiting")]
        [Authorize(Roles = "Moderator")]
        public async Task<IActionResult> GetAllWaitingPost(int? pageIndex)
        {
            var result = await _productPostService.ViewAllWaitingPost(pageIndex);
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
    }
}
