using BusinessObjects.Enums;
using BusinessObjects.Models;
using GoodsExchangeAtFUManagement.Repository.Repositories.CoinPackRepositories;
using GoodsExchangeAtFUManagement.Repository.Repositories.CoinTransactionRepositories;
using GoodsExchangeAtFUManagement.Repository.Repositories.UserRepositories;
using GoodsExchangeAtFUManagement.Service.Ultis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodsExchangeAtFUManagement.Service.Services.CoinTransactionServices
{
    public class CoinTransactionService : ICoinTransactionService
    {
        private readonly ICoinTransactionRepository _coinTransactionRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICoinPackRepository _coinPackRepository;

        public CoinTransactionService(ICoinTransactionRepository coinTransactionRepository, IUserRepository userRepository, ICoinPackRepository coinPackRepository)
        {
            _coinTransactionRepository = coinTransactionRepository;
            _userRepository = userRepository;
            _coinPackRepository = coinPackRepository;
        }

        public async Task<string> CreateCoinTransaction(string coinPackId, string token)
        {
            var userId = JwtGenerator.DecodeToken(token, "userId");
            var coinPack = await _coinPackRepository.GetSingle(c => c.Id.Equals(coinPackId));
            var newCoinTransaction = new CoinTransaction
            {
                Id = Guid.NewGuid().ToString(),
                TransactAt = DateTime.Now,
                Status = CoinTransactionStatus.Pending.ToString(),
                CoinPackId = coinPackId,
                UserId = userId,
                Price = coinPack.Price.Replace(".", "")
            };
            await _coinTransactionRepository.Insert(newCoinTransaction);

            return newCoinTransaction.Id;
        }

    }
}
