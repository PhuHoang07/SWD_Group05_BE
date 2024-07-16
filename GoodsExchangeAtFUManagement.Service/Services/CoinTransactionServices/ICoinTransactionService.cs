using BusinessObjects.DTOs.CoinTransactionDTOs;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
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
        Task<string> GetPaymentUrl(HttpContext context, string transactId, string redirectUrl);
        Task<CoinTransaction> UpdateCoinTransactionStatus(CoinTransactionUpdateReqModel reqModel);
    }
}
