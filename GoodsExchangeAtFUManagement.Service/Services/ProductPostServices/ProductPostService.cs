using AutoMapper;
using BusinessObjects.DTOs.ProductPostDTOs;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using GoodsExchangeAtFUManagement.Repository.Repositories.PostModeRepositories;
using GoodsExchangeAtFUManagement.Repository.Repositories.ProductPostRepositories;
using GoodsExchangeAtFUManagement.Service.Ultis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodsExchangeAtFUManagement.Service.Services.ProductPostServices
{
    public class ProductPostService : IProductPostService
    {
        private readonly IProductPostRepository _productPostRepository;
        private readonly IPostModeRepository _postModeRepository;
        private readonly IMapper _mapper;
        public ProductPostService(IProductPostRepository productPostRepository, IMapper mapper, IPostModeRepository postModeRepository)
        {
            _postModeRepository = postModeRepository;
            _productPostRepository = productPostRepository;
            _mapper = mapper;
        }

        public async Task CreateWaitingProductPost(ProductPostCreateRequestModel requestModel, string token)
        {
            ProductPost newProductPost = _mapper.Map<ProductPost>(requestModel);
            newProductPost.Id = Guid.NewGuid().ToString();
            newProductPost.Status = ProductPostStatus.Waiting.ToString();
            newProductPost.CreatedBy = JwtGenerator.DecodeToken(token, "userId");
            var chosenPostMode = await _postModeRepository.GetSingle(p => p.Id.Equals(requestModel.PostModeId));
            newProductPost.ExpiredDate = DateTime.Now.AddDays(int.Parse(chosenPostMode.Duration));
            await _productPostRepository.Insert(newProductPost);
        }
    }
}
