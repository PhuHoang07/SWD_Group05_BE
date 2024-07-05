using AutoMapper;
using BusinessObjects.DTOs.ProductPostDTOs;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using GoodsExchangeAtFUManagement.Repository.Repositories.PaymentRepositories;
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
        private readonly IPaymentRepository _paymentRepository;

        public ProductPostService(IProductPostRepository productPostRepository, IMapper mapper, IPostModeRepository postModeRepository,
            IProductImagesRepository productImagesRepository, IUserRepository userRepository, IPaymentRepository paymentRepository)
        {
            _postModeRepository = postModeRepository;
            _productPostRepository = productPostRepository;
            _productImagesRepository = productImagesRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _paymentRepository = paymentRepository;
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
            else
            {
                user.Balance -= int.Parse(chosenPostMode.Price);
            }
            var newPayment = new Payment
            {
                Id = Guid.NewGuid().ToString(),
                PaymentDate = DateTime.Now,
                Price = chosenPostMode.Price,
                ProductPostId = postId,
                PostModeId = requestModel.PostModeId
            };
            newProductPost.ExpiredDate = DateTime.Now.AddDays(int.Parse(chosenPostMode.Duration));
            await _productPostRepository.Insert(newProductPost);
            await _paymentRepository.Insert(newPayment);
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
            await _userRepository.Update(user);
            await _productImagesRepository.InsertRange(postImages);
        }

        public async Task<List<WaitingProductPostResponseModel>> ViewAllWaitingPost(int? pageIndex)
        {
            var allWaitingPost = await _productPostRepository.Get(p => p.Status.Equals(ProductPostStatus.Waiting.ToString()), null, includeProperties: "Category,PostMode,Campus,CreatedByNavigation", pageIndex ?? 1, 1);
            var waitingPostListId = allWaitingPost.Select(a => a.Id).ToList();
            var allImages = await _productImagesRepository.Get(i => waitingPostListId.Contains(i.ProductPostId));
            var responseList = allWaitingPost.Select(a => new WaitingProductPostResponseModel
            {
                Id = a.Id,
                Title = a.Title,
                Description = a.Description,
                Price = a.Price,
                CreatedBy = a.CreatedByNavigation.Id,
                Category = a.Category.Name,
                Campus = a.Campus.Name,
                ExpiredDate = a.ExpiredDate,
                PostMode = a.PostMode.Type,
                ImageUrls = allImages.Where(ai => ai.ProductPostId.Equals(a.Id)).Select(ai => ai.Url).ToList(),
            }).ToList();
            return responseList;
        }

        //public async Task
    }
}
