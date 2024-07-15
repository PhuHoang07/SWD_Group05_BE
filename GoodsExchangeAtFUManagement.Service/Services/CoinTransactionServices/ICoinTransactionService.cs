using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodsExchangeAtFUManagement.Service.Services.CoinTransactionServices
{
    public interface ICoinTransactionService
    {
        Task<string> CreateCoinTransaction(string coinPackId, string token);
    }
}
