using AutoMapper;
using BusinessObjects.DTOs.ProductPostDTOs;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using GoodsExchangeAtFUManagement.Repository.Repositories.PostModeRepositories;
using GoodsExchangeAtFUManagement.Repository.Repositories.ProductImagesRepositories;
using GoodsExchangeAtFUManagement.Repository.Repositories.ProductPostRepositories;
using GoodsExchangeAtFUManagement.Repository.Repositories.UserRepositories;
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
        private readonly IProductImagesRepository _productImagesRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public ProductPostService(IProductPostRepository productPostRepository, IMapper mapper, IPostModeRepository postModeRepository, IProductImagesRepository productImagesRepository, IUserRepository userRepository)
        {
            _postModeRepository = postModeRepository;
            _productPostRepository = productPostRepository;
            _productImagesRepository = productImagesRepository;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public async Task CreateWaitingProductPost(ProductPostCreateRequestModel requestModel, string token)
        {
            var userId = JwtGenerator.DecodeToken(token, "userId");
            var user = await _userRepository.GetSingle(u => u.Id.Equals(userId));
            ProductPost newProductPost = _mapper.Map<ProductPost>(requestModel);
            var postId = Guid.NewGuid().ToString();
            newProductPost.Id = postId;
            newProductPost.Status = ProductPostStatus.Waiting.ToString();
            newProductPost.CreatedBy = userId;
            var chosenPostMode = await _postModeRepository.GetSingle(p => p.Id.Equals(requestModel.PostModeId));
            if (int.Parse(chosenPostMode.Price) > user.Balance)
            {
                throw new CustomException("You dont have enough coin to choose this post mode. Please choose another post mode or recharge for more coins.");
            }
            newProductPost.ExpiredDate = DateTime.Now.AddDays(int.Parse(chosenPostMode.Duration));
            var postImages = new List<ProductImage>();
            foreach (var url in requestModel.ImagesUrl)
            {
                postImages.Add(new ProductImage
                {
                    Id = Guid.NewGuid().ToString(),
                    ProductPostId = postId,
                    Url = url
                });
            }
            await _productImagesRepository.InsertRange(postImages);
            await _productPostRepository.Insert(newProductPost);
        }

        public async Task ViewAllWaitingPost()
        {

        }
    }
}
