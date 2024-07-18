using BusinessObjects.DTOs.ProductPostDTOs;
using BusinessObjects.DTOs.ProductTransactionDTOs;

namespace GoodsExchangeAtFUManagement.Service.Services.ProductTransactionServices
{
    public interface IProductTransactionService
    {
        Task MakeProduct(string postId, string token);
        Task CancelBuyingPost(string id, string token);
        Task<List<ProductTransactionResponseModel>> ViewOwnBuyingProductWithStatus(int? pageIndex, string status, PostSearchModel searchModel, string token);
        Task<List<ProductTransactionInSellerViewModel>> ViewBuyerOfProductPost(string postId, int? pageIndex);
        Task<List<ProductTransactionResponseModel>> GetAllProduct(int? pageIndex, PostSearchModel searchModel);
    }
}
