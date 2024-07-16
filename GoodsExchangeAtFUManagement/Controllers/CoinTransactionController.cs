using BusinessObjects.DTOs.CoinTransactionDTOs;
using BusinessObjects.Enums;
using GoodsExchangeAtFUManagement.Service.Services.CoinTransactionServices;
using GoodsExchangeAtFUManagement.Service.Services.UserServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GoodsExchangeAtFUManagement.Controllers
{
    [Route("api/coin-transaction")]
    [ApiController]
    public class CoinTransactionController : ControllerBase
    {
        private readonly ICoinTransactionService _coinTransactionService;
        private readonly IUserService _userService;

        public CoinTransactionController(ICoinTransactionService coinTransactionService, IUserService userService)
        {
            _coinTransactionService = coinTransactionService;
            _userService = userService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllCoinTransaction(int? pageIndex)
        {
            var response = await _coinTransactionService.ViewAllCoinTransaction(pageIndex);
            return Ok(response);
        }

        [HttpGet]
        [Route("me")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetOwnCoinTransaction(int? pageIndex)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var response = await _coinTransactionService.ViewOwnCoinTransaction(token, pageIndex);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCoinTransaction(CoinTransactionCreateReqModel reqModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var transactId = await _coinTransactionService.CreateCoinTransaction(reqModel.coinPackId, token);
            var transactUrl = await _coinTransactionService.GetPaymentUrl(HttpContext, transactId, reqModel.redirectUrl);
            var response = new CoinTransactionPaymentResponseModel
            {
                transactId = transactId,
                transactUrl = transactUrl
            };

            return Ok(response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCoinTransaction(CoinTransactionUpdateReqModel reqModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var coinTransact = await _coinTransactionService.UpdateCoinTransactionStatus(reqModel);
            if (coinTransact.Status.Equals(CoinTransactionStatus.Success.ToString()))
            {
                await _userService.AddCoinToUserBalance(token, coinTransact.CoinPackId);
                return Ok("The transaction is successful. Update user balance successfully");
            }
            return Ok("The transaction is failed");
        }
    }
}
