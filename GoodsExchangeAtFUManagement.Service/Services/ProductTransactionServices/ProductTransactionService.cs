using BusinessObjects.DTOs.ProductPostDTOs;
using BusinessObjects.DTOs.ProductTransactionDTOs;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using GoodsExchangeAtFUManagement.Repository.Repositories.ProductImagesRepositories;
using GoodsExchangeAtFUManagement.Repository.Repositories.ProductPostRepositories;
using GoodsExchangeAtFUManagement.Repository.Repositories.ProductTransactionRepositories;
using GoodsExchangeAtFUManagement.Repository.Repositories.UserRepositories;
using GoodsExchangeAtFUManagement.Service.Ultis;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace GoodsExchangeAtFUManagement.Service.Services.ProductTransactionServices
{
    public class ProductTransactionService : IProductTransactionService
    {
        private readonly IProductTransactionRepository _productTransactionRepository;
        private readonly IProductPostRepository _productPostRepository;
        private readonly IProductImagesRepository _productImagesRepository;

        public ProductTransactionService(IProductTransactionRepository productTransactionRepository, IProductPostRepository productPostRepository, IProductImagesRepository productImagesRepository)
        {
            _productTransactionRepository = productTransactionRepository;
            _productPostRepository = productPostRepository;
            _productImagesRepository = productImagesRepository;
        }

        public async Task MakeProduct(string postId, string token)
        {
            var userId = JwtGenerator.DecodeToken(token, "userId");
            var chosenPost = await _productPostRepository.GetSingle(p => p.Id.Equals(postId));
            if (chosenPost == null)
            {
                throw new CustomException("The chosen post is not existed");
            }
            if (!chosenPost.Status.Equals(ProductPostStatus.Open.ToString()) && !chosenPost.Status.Equals(ProductPostStatus.Pending.ToString()))
            {
                throw new CustomException("This post is not open for buying");
            }
            chosenPost.Status = ProductPostStatus.Pending.ToString();
            var newPostTransaction = new ProductTransaction
            {
                Id = Guid.NewGuid().ToString(),
                Price = chosenPost.Price,
                TransactAt = DateTime.Now,
                Status = ProductTransactionStatus.Pending.ToString(),
                ProductPostId = postId,
                BuyerId = userId,
            };
            await _productTransactionRepository.Insert(newPostTransaction);
            await _productPostRepository.Update(chosenPost);
        }

        public async Task CancelBuyingPost(string id, string token)
        {
            var userId = JwtGenerator.DecodeToken(token, "userId");
            var chosenTransaction = await _productTransactionRepository.GetSingle(p => p.ProductPostId.Equals(id) && p.BuyerId.Equals(userId));
            if (chosenTransaction != null)
            {
                if (chosenTransaction.Status.Equals(ProductTransactionStatus.Success.ToString()))
                {
                    throw new CustomException("The product in this post is already bought by you. Cant delete post transaction!");
                }
                await _productTransactionRepository.Delete(chosenTransaction);
            }
            else throw new CustomException("This apply post is not existed");
        }

        public async Task<List<ProductTransactionResponseModel>> GetAllProduct(int? pageIndex, PostSearchModel searchModel)
        {
            Func<IQueryable<ProductPost>, IOrderedQueryable<ProductPost>> orderBy;
            orderBy = o => o.OrderBy(p => p.Price).ThenBy(p => p.CreatedDate);

            var buyingPost = await _productTransactionRepository.Get();
            var buyingPostIdList = buyingPost.Select(b => b.ProductPostId).ToList();
            Expression<Func<ProductPost, bool>> filter;

            filter = p => buyingPostIdList.Contains(p.Id);

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

            var responseList = allWaitingPost.Select(a =>
            {
                var transact = buyingPost.Where(p => p.ProductPostId.Equals(a.Id)).FirstOrDefault();
                return new ProductTransactionResponseModel
                {
                    Id = transact.Id,
                    TransactAt = transact.TransactAt,
                    Price = transact.Price,
                    responseModel = new ProductPostResponseModel
                    {
                        Id = a.Id,
                        Title = a.Title,
                        Description = a.Description,
                        Price = a.Price,
                        CreatedBy = new PostAuthor
                        {
                            FullName = a.CreatedByNavigation.Fullname,
                            Email = a.CreatedByNavigation.Email,
                            PhoneNumber = a.CreatedByNavigation.PhoneNumber
                        },
                        Category = a.Category.Name,
                        Campus = a.Campus.Name,
                        CreatedDate = a.CreatedDate,
                        ExpiredDate = a.ExpiredDate ?? null,
                        PostMode = a.PostMode.Type,
                        ImageUrls = allImages.Where(ai => ai.ProductPostId.Equals(a.Id)).Select(ai => ai.Url).ToList(),
                    }
                };
            }).ToList();
            return responseList;
        }

        public async Task<List<ProductTransactionResponseModel>> ViewOwnBuyingProductWithStatus(int? pageIndex, string status, PostSearchModel searchModel, string token)
        {
            return await ViewOwnBuyingPostWithStatus(pageIndex, status, searchModel, token);
        }

        public async Task<List<ProductTransactionInSellerViewModel>> ViewBuyerOfProductPost(string postId, int? pageIndex)
        {
            var transactList = await _productTransactionRepository.Get(p => p.ProductPostId.Equals(postId), null, includeProperties: "Buyer", pageIndex ?? 1, 5);
            return transactList.Select(a => new ProductTransactionInSellerViewModel
            {
                Id = a.Id,
                TransactAt = a.TransactAt,
                BuyerInfo = new BuyerInfo
                {
                    Id = a.BuyerId,
                    Email = a.Buyer.Email,
                    Name = a.Buyer.Fullname,
                    PhoneNumber = a.Buyer.PhoneNumber
                }
            }).ToList();
        }

        private async Task<List<ProductTransactionResponseModel>> ViewOwnBuyingPostWithStatus(int? pageIndex, string status, PostSearchModel searchModel, string token)
        {
            var userId = JwtGenerator.DecodeToken(token, "userId");
            Func<IQueryable<ProductPost>, IOrderedQueryable<ProductPost>> orderBy;
            orderBy = o => o.OrderBy(p => p.Price).ThenBy(p => p.CreatedDate);

            if (status.IsNullOrEmpty())
            {
                throw new CustomException("Please choose status");
            }

            if (!Enum.GetNames(typeof(ProductTransactionStatus)).Contains(status))
            {
                throw new CustomException("Please input valid status");
            }

            var buyingPost = await _productTransactionRepository.Get(p => p.BuyerId.Equals(userId) && p.Status.Equals(status));
            var buyingPostIdList = buyingPost.Select(b => b.ProductPostId).ToList();
            Expression<Func<ProductPost, bool>> filter;

            filter = p => buyingPostIdList.Contains(p.Id);

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

            var responseList = allWaitingPost.Select(a =>
            {
                var transact = buyingPost.Where(p => p.ProductPostId.Equals(a.Id)).FirstOrDefault();
                return new ProductTransactionResponseModel
                {
                    Id = transact.Id,
                    TransactAt = transact.TransactAt,
                    Price = transact.Price,
                    responseModel = new ProductPostResponseModel
                    {
                        Id = a.Id,
                        Title = a.Title,
                        Description = a.Description,
                        Price = a.Price,
                        CreatedBy = new PostAuthor
                        {
                            FullName = a.CreatedByNavigation.Fullname,
                            Email = a.CreatedByNavigation.Email,
                            PhoneNumber = a.CreatedByNavigation.PhoneNumber
                        },
                        Category = a.Category.Name,
                        Campus = a.Campus.Name,
                        CreatedDate = a.CreatedDate,
                        ExpiredDate = a.ExpiredDate ?? null,
                        PostMode = a.PostMode.Type,
                        ImageUrls = allImages.Where(ai => ai.ProductPostId.Equals(a.Id)).Select(ai => ai.Url).ToList(),
                    }
                };
            }).ToList();
            return responseList;
        }
    }
}

