using BusinessObjects.DTOs.PostModeDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodsExchangeAtFUManagement.Service.Services.PostModeServices
{
    public interface IPostModeService
    {
        Task CreatePostMode(PostModeCreateRequestModel requestModel);
        Task<List<PostModeResponseModel>> ViewActivePostMode();
        Task<List<PostModeManageResponseModel>> GetAllPost();
        Task<PostModeManageResponseModel> GetPostModeById(string id);
        Task UpdatePostMode(PostModeUpdateRequestModel requestModel, string id);
        Task SoftRemoveList(List<string> idList);
    }
}
