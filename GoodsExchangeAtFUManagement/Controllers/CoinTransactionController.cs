using GoodsExchangeAtFUManagement.Service.Services.CoinTransactionServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GoodsExchangeAtFUManagement.Controllers
{
    [Route("api/coin-transaction")]
    [ApiController]
    public class CoinTransactionController : ControllerBase
    {
        private readonly ICoinTransactionService _coinTransactionService;
        public CoinTransactionController(ICoinTransactionService coinTransactionService)
        {
            _coinTransactionService = coinTransactionService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCoinTransaction([FromBody] string coinPackId)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            await _coinTransactionService.CreateCoinTransaction(coinPackId, token);
            return Ok("Create coin transaction successfully");
        }


    }
}
