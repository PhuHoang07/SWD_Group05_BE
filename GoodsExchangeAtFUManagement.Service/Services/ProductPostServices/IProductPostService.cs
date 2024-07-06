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
        Task ApprovePost(string status, string id);
        Task<List<ProductPostResponseModel>> ViewAllPostWithStatus(int? pageIndex, PostSearchModel searchModel, string status);
        Task<List<ProductPostResponseModel>> ViewOwnPostWithStatus(int? pageIndex, PostSearchModel searchModel, string status, string token);

    }
}
