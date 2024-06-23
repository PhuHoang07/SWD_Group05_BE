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

        public async Task<CoinPackManageResponseModel> GetCoinPackById(string id)
        {
            var coinPack = await _coinPackRepository.GetSingle(c => c.Id.Equals(id));
            if (coinPack == null)
            {
                throw new CustomException("The chosen coin pack is not existed");
            }
            var coinPackInfo = _mapper.Map<CoinPackManageResponseModel>(coinPack);
            return coinPackInfo;
        }

        public async Task UpdateCoinPack(CoinPackUpdateRequestModel requestModel, string id)
        {
            if (id == null)
            {
                throw new CustomException("Please input id for update");
            }
            var coinPack = await _coinPackRepository.GetSingle(c => c.Id.Equals(id));
            if (coinPack == null)
            {
                throw new CustomException("Cant find the coin pack");
            }
            _mapper.Map(requestModel, coinPack);
            await _coinPackRepository.Update(coinPack);
        }

        public async Task SoftRemoveCoinPack(List<string> listId)
        {
            var coinPackList = await _coinPackRepository.Get(c => listId.Contains(c.Id));
            foreach (var coinPack in coinPackList)
            {
                if (coinPack.Status.Equals(CoinPackStatus.Inactive.ToString()))
                {
                    throw new CustomException("The chosen coin pack is already removed");
                }
                else
                {
                    coinPack.Status = CoinPackStatus.Inactive.ToString();
                }
            }
            await _coinPackRepository.UpdateRange(coinPackList.ToList());
        }
    }
}
