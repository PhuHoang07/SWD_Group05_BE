using BusinessObjects.DTOs.ProductPostDTOs;
using BusinessObjects.DTOs.ProductTransactionDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodsExchangeAtFUManagement.Service.Services.ProductTransactionServices
{
    public interface IProductTransactionService
    {
        Task BuyProductPost(string postId, string token);
        Task CancelBuyingPost(string id);
        Task<List<ProductTransactionResponseModel>> ViewOwnBuyingProductWithStatus(int? pageIndex, string status, PostSearchModel searchModel, string token);
        Task<List<ProductTransactionInSellerViewModel>> ViewBuyerOfProductPost(string postId, int? pageIndex);
    }
}
