﻿using BusinessObjects.DTOs.ProductPostDTOs;
using GoodsExchangeAtFUManagement.Service.Services.ProductPostServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

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
        public async Task<IActionResult> FetchAllProduct(int? pageIndex, [FromQuery] PostSearchModel searchModel, string? status)
        {
            var result = await _productPostService.GetAllProduct(pageIndex, searchModel, status);
            return Ok(result);
        }

        [HttpGet]
        [Route("me")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetOwnProductPost(int? pageIndex, [FromQuery] PostSearchModel searchModel, string? status)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var result = await _productPostService.ViewOwnPostWithStatus(pageIndex, searchModel, status, token);
            return Ok(result);
        }

        [HttpGet]
        [Route("others")]
        public async Task<IActionResult> GetOthersProductPost(int? pageIndex, [FromQuery] PostSearchModel searchModel)
        {
            var authorize = Request.Headers["Authorization"].ToString().Split(" ");
            var result = new List<ProductPostResponseModel>();
            if (authorize.Length > 1)
            {
                var token = authorize[1];
                result = await _productPostService.ViewOwnPostExceptMine(pageIndex, searchModel, token);
            }
            else
            {
                result = await _productPostService.ViewOwnPostExceptMine(pageIndex, searchModel, null);
            }
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetDetailsOfPost(string id)
        {
            var response = await _productPostService.ViewDetailsOfPost(id);
            return Ok(response);
        }

        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> UpdateProductPost(string id, ProductPostUpdateRequestModel requestModel)
        {
            await _productPostService.UpdateProductPost(id, requestModel);
            return Ok("Update successfully");
        }

        [HttpPut]
        [Route("extend/{id}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> ExtendProductPost(string id, [FromBody] string postModeId)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            await _productPostService.ExtendExpiredDate(id, postModeId, token);
            return Ok("Update successfully");
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> MakeProduct(ProductPostCreateRequestModel requestModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            await _productPostService.MakeProduct(requestModel, token);
            return Ok("Create post successfully. Please wait for moderator approving your post");
        }

        [HttpPut]
        [Route("approve/{id}")]
        [Authorize(Roles = "Moderator")]
        public async Task<IActionResult> ApproveProductPost([FromBody] string status, string id)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            await _productPostService.ApprovePost(status, id, token);
            return Ok("Approve post successfully");
        }

        [HttpPut]
        [Route("close/{id}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CloseProductPost(string id, [FromBody] string postApplyId)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            await _productPostService.ClosePost(id, token, postApplyId);
            return Ok("Close post successfully");
        }

        [HttpGet]
        [Route("{id}/payment-records")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetPostPaymentRecords(string id, int? pageIndex)
        {
            var result = await _productPostService.GetPostPaymentRecords(pageIndex, id);
            return Ok(result);
        }
    }
}
