using BusinessObjects.DTOs.CategoryDTOs;
using BusinessObjects.DTOs.CoinPackDTOs;
using GoodsExchangeAtFUManagement.Service.Services.CoinPackServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GoodsExchangeAtFUManagement.Controllers
{
    [Route("api/coin-pack")]
    [ApiController]
    public class CoinPackController : ControllerBase
    {
        private readonly ICoinPackService _coinPackService;

        public CoinPackController(ICoinPackService coinPackService)
        {
            _coinPackService = coinPackService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("create")]
        public async Task<IActionResult> CreateCoinPack(CoinPackCreateRequestModel request)
        {
            await _coinPackService.CreateCoinPack(request);
            return Ok("Coin pack created successfully!");
        }

        [HttpGet]
        [Route("view/active")]
        public async Task<IActionResult> ViewActiveCoinPacks()
        {
            var coinList = await _coinPackService.ViewActiveListCoinPack();
            return Ok(coinList);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("view/all")]
        public async Task<IActionResult> ViewAllCoinPacks()
        {
            var coinList = await _coinPackService.ViewAllListCoinPack();
            return Ok(coinList);
        }

        [HttpGet]
        [Authorize]
        [Route("view/{id}")]
        public async Task<IActionResult> ViewCoinPackById(string id)
        {
            var coinPack = await _coinPackService.GetCoinPackById(id);
            return Ok(coinPack);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        [Route("update/{id}")]
        public async Task<IActionResult> UpdateCoinPacks(CoinPackUpdateRequestModel requestModel, string id)
        {
            await _coinPackService.UpdateCoinPack(requestModel, id);
            return Ok("Coin pack update successfully");
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        [Route("soft-remove")]
        public async Task<IActionResult> SoftRemoveCoinPacks(List<string> id)
        {
            await _coinPackService.SoftRemoveCoinPack(id);
            return Ok("Coin pack(s) remove successfully");
        }

    }
}
