using BusinessObjects.DTOs.ProductPostDTOs;
using GoodsExchangeAtFUManagement.Service.Services.ProductTransactionServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GoodsExchangeAtFUManagement.Controllers
{
    [Route("api/product-transaction")]
    [ApiController]
    public class ProductTransactionController : ControllerBase
    {
        private readonly IProductTransactionService _productTransactionService;
        public ProductTransactionController(IProductTransactionService productTransactionService)
        {
            _productTransactionService = productTransactionService;
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        [Route("{postId}")]
        public async Task<IActionResult> BuyProduct(string postId)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            await _productTransactionService.BuyProductPost(postId, token);
            return Ok("Buy successfully");
        }

        [HttpDelete]
        [Authorize(Roles = "User")]
        [Route("{postId}")]
        public async Task<IActionResult> CancelBuyProduct(string postId)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            await _productTransactionService.CancelBuyingPost(postId);
            return Ok("Cancel successfully");
        }

        [HttpGet]
        [Authorize(Roles = "User")]
        [Route("me")]
        public async Task<IActionResult> ViewOwnBuyingProductWithStatus(int? pageIndex, string status, [FromQuery] PostSearchModel searchModel)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var response = await _productTransactionService.ViewOwnBuyingProductWithStatus(pageIndex, status, searchModel, token);
            return Ok(response);
        }

        [HttpGet]
        [Authorize(Roles = "User")]
        [Route("{postId}/buyers")]
        public async Task<IActionResult> ViewBuyerOfProductPost(string postId, int? pageIndex)
        {
            var response = await _productTransactionService.ViewBuyerOfProductPost(postId, pageIndex);
            return Ok(response);
        }
    }
}
