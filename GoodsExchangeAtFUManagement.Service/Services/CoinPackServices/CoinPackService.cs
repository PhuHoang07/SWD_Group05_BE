using AutoMapper;
using BusinessObjects.DTOs.CoinPackDTOs;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using GoodsExchangeAtFUManagement.Repository.Repositories.CoinPackRepositories;
using GoodsExchangeAtFUManagement.Service.Ultis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodsExchangeAtFUManagement.Service.Services.CoinPackServices
{
    public class CoinPackService : ICoinPackService
    {
        private readonly ICoinPackRepository _coinPackRepository;
        private readonly IMapper _mapper;
        public CoinPackService(ICoinPackRepository coinPackRepository, IMapper mapper)
        {
            _coinPackRepository = coinPackRepository;
            _mapper = mapper;
        }

        public async Task CreateCoinPack(CoinPackCreateRequestModel request)
        {
            CoinPack newCoinPack = _mapper.Map<CoinPack>(request);
            newCoinPack.Id = Guid.NewGuid().ToString();
            newCoinPack.Status = CoinPackStatus.Active.ToString();
            newCoinPack.CreatedAt = DateTime.Now;
            await _coinPackRepository.Insert(newCoinPack);
        }

        public async Task<List<CoinPackResponseModel>> ViewActiveListCoinPack()
        {
            var coinList = await _coinPackRepository.Get(c => c.Status.Equals(CoinPackStatus.Active.ToString()));
            var showList = _mapper.Map<List<CoinPackResponseModel>>(coinList);
            return showList;
        }
        
        public async Task<List<CoinPackManageResponseModel>> ViewAllListCoinPack()
        {
            var coinList = await _coinPackRepository.Get();
            var showList = _mapper.Map<List<CoinPackManageResponseModel>>(coinList);
            return showList;
        }

        public async Task<CoinPackResponseModel> GetCoinPackById(string id)
        {
            var coinPack = await _coinPackRepository.GetSingle(c => c.Id.Equals(id));
            var coinPackInfo = _mapper.Map<CoinPackResponseModel>(coinPack);
            return coinPackInfo;
        }

        public async Task UpdateCoinPack(CoinPackUpdateRequestModel requestModel)
        {
            var coinPack = await _coinPackRepository.GetSingle(c => c.Id.Equals(requestModel.Id));
            if (coinPack == null)
            {
                throw new CustomException("Cant find the coin pack");
            }
            coinPack.CoinAmount = requestModel.CoinAmount;
            coinPack.Price = requestModel.Price;
            await _coinPackRepository.Update(coinPack);
        }

        public async Task SoftRemoveCoinPack(string id)
        {
            var coinPack = await _coinPackRepository.GetSingle(c => c.Id.Equals(id));
            if (coinPack == null)
            {
                throw new CustomException("Cant find the coin pack");
            }
            if (coinPack.Status.Equals(CoinPackStatus.Inactive.ToString()))
            {
                throw new CustomException("This coin pack is already be removed");
            }
            coinPack.Status = CoinPackStatus.Inactive.ToString();
            await _coinPackRepository.Update(coinPack);
        }
    }
}
