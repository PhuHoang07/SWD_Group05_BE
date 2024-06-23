using BusinessObjects.DTOs.CoinPackDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodsExchangeAtFUManagement.Service.Services.CoinPackServices
{
    public interface ICoinPackService
    {
        Task CreateCoinPack(CoinPackCreateRequestModel request);
        Task<List<CoinPackResponseModel>> ViewActiveListCoinPack();
        Task<List<CoinPackManageResponseModel>> ViewAllListCoinPack();
        Task<CoinPackResponseModel> GetCoinPackById(string id);
        Task UpdateCoinPack(CoinPackUpdateRequestModel requestModel);
        Task SoftRemoveCoinPack(string id);
    }
}
