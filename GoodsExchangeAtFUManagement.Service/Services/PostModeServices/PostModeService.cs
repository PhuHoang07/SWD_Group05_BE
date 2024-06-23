using AutoMapper;
using BusinessObjects.DTOs.PostModeDTOs;
using BusinessObjects.Models;
using GoodsExchangeAtFUManagement.Repository.Repositories.PostModeRepositories;
using GoodsExchangeAtFUManagement.Service.Ultis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodsExchangeAtFUManagement.Service.Services.PostModeServices
{
    public class PostModeService : IPostModeService
    {
        private readonly IPostModeRepository _postModeRepository;
        private readonly IMapper _mapper;

        public PostModeService(IPostModeRepository postModeRepository, IMapper mapper)
        {
            _postModeRepository = postModeRepository;
            _mapper = mapper;
        }

        public async Task CreatePostMode(PostModeCreateRequestModel requestModel)
        {
            var postModeList = await _postModeRepository.Get(p => p.Status == true);
            foreach (var postMode in postModeList)
            {
                if (postMode.Type.ToLower().Equals(requestModel.Type.ToLower()))
                {
                    throw new CustomException("This type name is already used");
                }
            }

            PostMode newPostMode = _mapper.Map<PostMode>(requestModel);
            newPostMode.Id = Guid.NewGuid().ToString();
            newPostMode.Status = true;
            await _postModeRepository.Insert(newPostMode);
        }

        public async Task<List<PostModeResponseModel>> ViewActivePostMode()
        {
            var list = await _postModeRepository.Get(p => p.Status == true);
            return _mapper.Map<List<PostModeResponseModel>>(list);
        }

        public async Task<List<PostModeManageResponseModel>> ViewAllPostMode()
        {
            var list = await _postModeRepository.Get();
            return _mapper.Map<List<PostModeManageResponseModel>>(list);
        }

        public async Task<PostModeManageResponseModel> GetPostModeById(string id)
        {
            var postMode = await _postModeRepository.GetSingle(p => p.Id.Equals(id));
            if (postMode == null)
            {
                throw new CustomException("The chosen post mode is not existed");
            }
            return _mapper.Map<PostModeManageResponseModel>(postMode);
        }

        public async Task UpdatePostMode(PostModeUpdateRequestModel requestModel, string id)
        {
            if (id == null)
            {
                throw new CustomException("Please input id for update");
            }
            var postMode = await _postModeRepository.GetSingle(p => p.Id.Equals(id));
            if (postMode == null)
            {
                throw new CustomException("The chosen post mode is not existed");
            }
            _mapper.Map(requestModel, postMode);
            await _postModeRepository.Update(postMode);
        }

        public async Task SoftRemoveList(List<string> idList)
        {
            var removeList = await _postModeRepository.Get(p => idList.Contains(p.Id));
            foreach (var postMode in removeList)
            {
                if (postMode.Status == false)
                {
                    throw new CustomException("The chosen coin pack is already removed");
                }
                else
                {
                    postMode.Status = false;
                }
            }
            await _postModeRepository.UpdateRange(removeList.ToList());
        }
    }
}
