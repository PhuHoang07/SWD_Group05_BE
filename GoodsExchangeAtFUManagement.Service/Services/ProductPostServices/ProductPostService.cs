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
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
            newProductPost.CreatedDate = DateTime.Now;
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

        public async Task<List<ProductPostResponseModel>> ViewAllPostWithStatus(int? pageIndex, PostSearchModel searchModel, string status)
        {
            return await ViewAllPostWithStatus(pageIndex, status, searchModel, null);
        }

        public async Task<List<ProductPostResponseModel>> ViewOwnPostWithStatus(int? pageIndex, PostSearchModel searchModel, string status, string token)
        {
            return await ViewAllPostWithStatus(pageIndex, status, searchModel, token);
        }

        public async Task ApprovePost(string status, string id)
        {
            if (!status.Equals(ProductPostStatus.Cancel.ToString()) && !status.Equals(ProductPostStatus.Open.ToString()))
            {
                throw new CustomException("Please input valid status");
            }
            var chosenPost = await _productPostRepository.GetSingle(a => a.Id.Equals(id));

            if (chosenPost != null)
            {
                if (!chosenPost.Status.Equals(ProductPostStatus.Waiting.ToString()))
                {
                    throw new CustomException("This post is not in waiting status");
                }
                chosenPost.Status = status;
            }
            else throw new CustomException("There is no existed post with chosen Id");

            var postMode = await _postModeRepository.GetSingle(p => p.Id.Equals(chosenPost.PostModeId));
            if (status.Equals(ProductPostStatus.Open.ToString()))
            {
                chosenPost.ExpiredDate = DateTime.Now.AddDays(int.Parse(postMode.Duration));
            }
            await _productPostRepository.Update(chosenPost);
        }

        private async Task<List<ProductPostResponseModel>> ViewAllPostWithStatus(int? pageIndex, string status, PostSearchModel searchModel, string token)
        {
            var userId = JwtGenerator.DecodeToken(token, "userId");
            Func<IQueryable<ProductPost>, IOrderedQueryable<ProductPost>> orderBy;
            orderBy = o => o.OrderBy(p => p.Price).ThenBy(p => p.CreatedDate);
            Expression<Func<ProductPost, bool>> filter;
            if (status.IsNullOrEmpty())
            {
                throw new CustomException("Please choose status");
            }

            if (!Enum.GetNames(typeof(ProductPostStatus)).Contains(status))
            {
                throw new CustomException("Please input valid status");
            }

            filter = p => p.Status.Equals(status);

            if (token != null)
            {
                filter = filter.And(p => p.CreatedBy.Equals(userId));
            }

            if (searchModel != null)
            {
                if (searchModel.orderPriceDescending.HasValue && searchModel.orderPriceDescending.Value)
                {
                    orderBy = orderBy.AndThen(q => q.OrderByDescending(p => p.Price));
                }
                else if (searchModel.orderPriceDescending.HasValue && !searchModel.orderPriceDescending.Value)
                {
                    orderBy = orderBy.AndThen(q => q.OrderBy(p => p.Price));
                }

                if (searchModel.orderDateDescending.HasValue && searchModel.orderDateDescending.Value)
                {
                    orderBy = orderBy.AndThen(q => q.OrderByDescending(p => p.CreatedDate));
                }
                else if (searchModel.orderDateDescending.HasValue && !searchModel.orderDateDescending.Value)
                {
                    orderBy = orderBy.AndThen(q => q.OrderBy(p => p.CreatedDate));
                }

                if (!searchModel.Campus.IsNullOrEmpty())
                {
                    filter = filter.And(p => p.Campus.Name.ToLower().Equals(searchModel.Campus.ToLower()));
                }
                if (!searchModel.Title.IsNullOrEmpty())
                {
                    filter = filter.And(p => p.Title.ToLower().Contains(searchModel.Title.ToLower()));
                }
                if (!searchModel.Category.IsNullOrEmpty())
                {
                    filter = filter.And(p => p.Category.Name.ToLower().Contains(searchModel.Category.ToLower()));
                }
            }

            var allWaitingPost = await _productPostRepository.Get(filter, orderBy, includeProperties: "Category,PostMode,Campus,CreatedByNavigation", pageIndex ?? 1, 3);
            var waitingPostListId = allWaitingPost.Select(a => a.Id).ToList();
            var allImages = await _productImagesRepository.Get(i => waitingPostListId.Contains(i.ProductPostId));

            var responseList = allWaitingPost.Select(a => new ProductPostResponseModel
            {
                Id = a.Id,
                Title = a.Title,
                Description = a.Description,
                Price = a.Price,
                CreatedBy = a.CreatedByNavigation.Id,
                Category = a.Category.Name,
                Campus = a.Campus.Name,
                CreatedDate = a.CreatedDate,
                ExpiredDate = a.ExpiredDate ?? null,
                PostMode = a.PostMode.Type,
                ImageUrls = allImages.Where(ai => ai.ProductPostId.Equals(a.Id)).Select(ai => ai.Url).ToList(),
            }).ToList();
            return responseList;
        }
    }
}
