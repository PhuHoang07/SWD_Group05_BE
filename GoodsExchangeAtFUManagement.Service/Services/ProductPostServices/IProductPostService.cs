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
        Task<List<WaitingProductPostResponseModel>> ViewAllWaitingPost(int? pageIndex);
    }
}
