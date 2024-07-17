using BusinessObjects.DTOs.PaymentDTOs;
using BusinessObjects.DTOs.ProductPostDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodsExchangeAtFUManagement.Service.Services.ProductPostServices
{
    public interface IProductPostService
    {
        Task CreateWaitingProductPost(ProductPostCreateRequestModel requestModel, string token);
        Task ApprovePost(string status, string id, string token);
        Task<List<ProductPostResponseModel>> ViewAllPostWithStatus(int? pageIndex, PostSearchModel searchModel, string status);
        Task<List<ProductPostResponseModel>> ViewOwnPostWithStatus(int? pageIndex, PostSearchModel searchModel, string status, string token);
        Task<List<ProductPostResponseModel>> ViewOwnPostExceptMine(int? pageIndex, PostSearchModel searchModel, string token);
        Task<ProductPostResponseModel> ViewDetailsOfPost(string id);
        Task UpdateProductPost(string id, ProductPostUpdateRequestModel requestModel);
        Task ExtendExpiredDate(string id, string postModeId, string token);
        Task ExtendExpiredDateAfterPaymentSuccess(string id, string postModeId);
        Task ClosePost(string id, string token, string postApplyId);
        Task<List<PaymentResponseModel>> GetPostPaymentRecords(int? pageIndex, string postId);
    }
}
