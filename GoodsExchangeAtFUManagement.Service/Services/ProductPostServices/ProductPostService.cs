using AutoMapper;
using BusinessObjects.DTOs.PaymentDTOs;
using BusinessObjects.DTOs.ProductPostDTOs;
using BusinessObjects.DTOs.VnPayDTOs;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using GoodsExchangeAtFUManagement.Repository.Repositories.PaymentRepositories;
using GoodsExchangeAtFUManagement.Repository.Repositories.PostModeRepositories;
using GoodsExchangeAtFUManagement.Repository.Repositories.ProductImagesRepositories;
using GoodsExchangeAtFUManagement.Repository.Repositories.ProductPostRepositories;
using GoodsExchangeAtFUManagement.Repository.Repositories.ProductTransactionRepositories;
using GoodsExchangeAtFUManagement.Repository.Repositories.UserRepositories;
using GoodsExchangeAtFUManagement.Service.Ultis;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;
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
        private readonly IProductTransactionRepository _productTransactionRepository;

        private const int ItemPerPage = 8;

        public ProductPostService(IProductPostRepository productPostRepository, IMapper mapper, IPostModeRepository postModeRepository,
            IProductImagesRepository productImagesRepository, IUserRepository userRepository, IPaymentRepository paymentRepository, IProductTransactionRepository productTransactionRepository)
        {
            _postModeRepository = postModeRepository;
            _productPostRepository = productPostRepository;
            _productImagesRepository = productImagesRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _paymentRepository = paymentRepository;
            _productTransactionRepository = productTransactionRepository;
        }

        public async Task MakeProduct(ProductPostCreateRequestModel requestModel, string token)
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

        public async Task<List<ProductPostResponseModel>> GetAllProduct(int? pageIndex, PostSearchModel searchModel, string status)
        {
            return await ViewAllPostWithStatus(pageIndex, status, searchModel, null, 0);
        }

        public async Task<List<ProductPostResponseModel>> ViewOwnPostWithStatus(int? pageIndex, PostSearchModel searchModel, string status, string token)
        {
            return await ViewAllPostWithStatus(pageIndex, status, searchModel, token, 1);
        }

        public async Task<List<ProductPostResponseModel>> ViewOwnPostExceptMine(int? pageIndex, PostSearchModel searchModel, string token)
        {
            string userId = null;
            if (token != null)
            {
                userId = JwtGenerator.DecodeToken(token, "userId");
            }
            Func<IQueryable<ProductPost>, IOrderedQueryable<ProductPost>> orderBy;
            orderBy = o => o.OrderBy(p => p.Price).ThenBy(p => p.CreatedDate);
            Expression<Func<ProductPost, bool>> filter;

            filter = p => p.Status.Equals(ProductPostStatus.Open.ToString()) || p.Status.Equals(ProductPostStatus.Pending.ToString());

            if (userId != null)
            {
                filter = filter.And(p => !p.CreatedBy.Equals(userId));
            }

            var listPostApply = await _productTransactionRepository.Get(p => p.BuyerId.Equals(userId));
            var exceptList = listPostApply.Select(l => l.ProductPostId).ToList();
            if (exceptList.Count() > 0)
            {
                filter = filter.And(p => !exceptList.Contains(p.Id));
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

            var allWaitingPost = await _productPostRepository.Get(filter, orderBy, includeProperties: "Category,PostMode,Campus,CreatedByNavigation", pageIndex ?? 1, ItemPerPage);
            var waitingPostListId = allWaitingPost.Select(a => a.Id).ToList();
            var allImages = await _productImagesRepository.Get(i => waitingPostListId.Contains(i.ProductPostId));

            var responseList = allWaitingPost.Select(a => new ProductPostResponseModel
            {
                Id = a.Id,
                Title = a.Title,
                Description = a.Description,
                Price = a.Price,
                Status = a.Status,
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
            }).ToList();
            return responseList;
        }

        public async Task<ProductPostResponseModel> ViewDetailsOfPost(string id)
        {
            var chosenPost = await _productPostRepository.GetSingle(p => p.Id.Equals(id), includeProperties: "Category,PostMode,Campus,CreatedByNavigation");
            if (chosenPost == null)
            {
                throw new CustomException("The chosen post is not existed");
            }
            var allImages = await _productImagesRepository.Get(i => i.ProductPostId.Equals(id));
            return new ProductPostResponseModel
            {
                Id = chosenPost.Id,
                Title = chosenPost.Title,
                Description = chosenPost.Description,
                Price = chosenPost.Price,
                Status = chosenPost.Status,
                CreatedBy = new PostAuthor
                {
                    FullName = chosenPost.CreatedByNavigation.Fullname,
                    Email = chosenPost.CreatedByNavigation.Email,
                    PhoneNumber = chosenPost.CreatedByNavigation.PhoneNumber
                },
                Category = chosenPost.Category.Name,
                Campus = chosenPost.Campus.Name,
                CreatedDate = chosenPost.CreatedDate,
                ExpiredDate = chosenPost.ExpiredDate ?? null,
                PostMode = chosenPost.PostMode.Type,
                ImageUrls = allImages.Select(ai => ai.Url).ToList(),
            };
        }

        public async Task UpdateProductPost(string id, ProductPostUpdateRequestModel requestModel)
        {
            var chosenPost = await _productPostRepository.GetSingle(p => p.Id.Equals(id));
            if (chosenPost == null)
            {
                throw new CustomException("The chosen post is not existed");
            }

            if (chosenPost.Status.Equals(ProductPostStatus.Pending.ToString()) || chosenPost.Status.Equals(ProductPostStatus.Closed.ToString()))
            {
                throw new CustomException("The chosen post cannot be edited");
            }
            var allPostImages = await _productImagesRepository.Get(i => i.ProductPostId.Equals(id));
            var imageUrls = allPostImages.Select(i => i.Url).ToList();
            if (requestModel != null)
            {
                _mapper.Map(requestModel, chosenPost);
            }

            var exceptImageUrl = imageUrls.Except(requestModel.ImagesUrl);
            if (exceptImageUrl.Count() > 0)
            {
                var deleteImage = allPostImages.Where(i => exceptImageUrl.Contains(i.Url)).ToList();
                await _productImagesRepository.DeleteRange(deleteImage);
            }

            var addedImageUrl = requestModel.ImagesUrl.Except(imageUrls);
            var addImageList = new List<ProductImage>();
            if (addedImageUrl.Count() > 0)
            {
                foreach (var url in addedImageUrl)
                {
                    addImageList.Add(new ProductImage
                    {
                        Id = Guid.NewGuid().ToString(),
                        ProductPostId = id,
                        Url = url
                    });
                }
                await _productImagesRepository.InsertRange(addImageList);
            }
        }

        public async Task ExtendExpiredDate(string id, string postModeId, string token)
        {
            var userId = JwtGenerator.DecodeToken(token, "userId");
            var user = await _userRepository.GetSingle(u => u.Id.Equals(userId));
            var chosenPost = await _productPostRepository.GetSingle(p => p.Id.Equals(id));
            if (chosenPost == null)
            {
                throw new CustomException("The chosen post is not existed");
            }

            if (!chosenPost.Status.Equals(ProductPostStatus.Expired.ToString()))
            {
                throw new CustomException("The chosen post cannot be extended");
            }

            var chosenPostMode = await _postModeRepository.GetSingle(p => p.Id.Equals(postModeId));

            if (int.Parse(chosenPostMode.Price) > user.Balance)
            {
                throw new CustomException("You dont have enough coin to choose this post mode. Please choose another post mode or recharge for more coins.");
            }
            else
            {
                user.Balance -= int.Parse(chosenPostMode.Price);
                chosenPost.Status = ProductPostStatus.Open.ToString();
            }
            var newPayment = new Payment
            {
                Id = Guid.NewGuid().ToString(),
                PaymentDate = DateTime.Now,
                Price = chosenPostMode.Price,
                ProductPostId = id,
                PostModeId = postModeId
            };
            await _paymentRepository.Insert(newPayment);
            await _userRepository.Update(user);
            chosenPost.ExpiredDate = chosenPost.ExpiredDate.Value.AddDays(int.Parse(chosenPostMode.Duration));
            chosenPost.PostModeId = postModeId;
            await _productPostRepository.Update(chosenPost);
            //await CreateAutoUpdateExpired(chosenPost);
        }

        public async Task ApprovePost(string status, string id, string token)
        {
            var userId = JwtGenerator.DecodeToken(token, "userId");
            var user = await _userRepository.GetSingle(u => u.Id.Equals(userId));
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
            else
            {
                user.Balance += int.Parse(postMode.Price);
            }
            await _productPostRepository.Update(chosenPost);
            //if (chosenPost.ExpiredDate != null)
            //{
            //    await CreateAutoUpdateExpired(chosenPost);
            //}
        }

        public async Task ClosePost(string id, string token, string postApplyId)
        {
            var chosenPost = await _productPostRepository.GetSingle(a => a.Id.Equals(id));
            var userId = JwtGenerator.DecodeToken(token, "userId");

            if (chosenPost != null)
            {
                if (!chosenPost.Status.Equals(ProductPostStatus.Pending.ToString()))
                {
                    throw new CustomException("This post is not in pending status");
                }

                if (!chosenPost.CreatedBy.Equals(userId))
                {
                    throw new CustomException("This post is not created by you, so you cant close it");
                }

                chosenPost.Status = ProductPostStatus.Closed.ToString();
            }
            else throw new CustomException("There is no existed post with chosen Id");
            var deletePostApplyList = await _productTransactionRepository.Get(p => !p.Id.Equals(postApplyId) && p.ProductPostId.Equals(id));
            var chosenPostApply = await _productTransactionRepository.GetSingle(p => p.Id.Equals(postApplyId));
            chosenPostApply.Status = ProductTransactionStatus.Success.ToString();

            await _productTransactionRepository.DeleteRange(deletePostApplyList.ToList());
            await _productPostRepository.Update(chosenPost);
            await _productTransactionRepository.Update(chosenPostApply);

        }

        private async Task<List<ProductPostResponseModel>> ViewAllPostWithStatus(int? pageIndex, string? status, PostSearchModel searchModel, string token, int option)
        {
            string userId = null;
            if (token != null)
            {
                userId = JwtGenerator.DecodeToken(token, "userId");
            }
            Func<IQueryable<ProductPost>, IOrderedQueryable<ProductPost>> orderBy;
            orderBy = o => o.OrderBy(p => p.Price).ThenBy(p => p.CreatedDate);
            Expression<Func<ProductPost, bool>> filter = p => true;
            if (!status.IsNullOrEmpty())
            {
                if (!Enum.GetNames(typeof(ProductPostStatus)).Contains(status))
                {
                    throw new CustomException("Please input valid status");
                }
                filter = p => p.Status.Equals(status);
            }

            if (userId != null && option == 1)
            {
                filter = filter.And(p => p.CreatedBy.Equals(userId));
            }
            else if (userId != null && option == 0)
            {
                filter = filter.And(p => !p.CreatedBy.Equals(userId));
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

            var allWaitingPost = await _productPostRepository.Get(filter, orderBy, includeProperties: "Category,PostMode,Campus,CreatedByNavigation", pageIndex ?? 1, ItemPerPage);
            var waitingPostListId = allWaitingPost.Select(a => a.Id).ToList();
            var allImages = await _productImagesRepository.Get(i => waitingPostListId.Contains(i.ProductPostId));

            var responseList = allWaitingPost.Select(a => new ProductPostResponseModel
            {
                Id = a.Id,
                Title = a.Title,
                Description = a.Description,
                Price = a.Price,
                Status = a.Status,
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
            }).ToList();
            return responseList;
        }

        public async Task ExtendExpiredDateAfterPaymentSuccess(string id, string postModeId)
        {
            var chosenPost = await _productPostRepository.GetSingle(p => p.Id.Equals(id));
            if (chosenPost == null)
            {
                throw new CustomException("The chosen post is not existed");
            }

            var chosenPostMode = await _postModeRepository.GetSingle(p => p.Id.Equals(postModeId));

            if (chosenPost.Status.Equals(ProductPostStatus.Unpaid.ToString()))
            {
                // waiting means wait for admin approval
                chosenPost.Status = ProductPostStatus.Waiting.ToString();
            }
            else if (chosenPost.Status.Equals(ProductPostStatus.Expired.ToString()))
            {
                // open means post that approved but is expired and then extend
                chosenPost.Status = ProductPostStatus.Open.ToString();
            }
            else
            {
                throw new Exception($"Post status is {chosenPost.Status} and is not allowed to update!");
            }

            chosenPost.ExpiredDate = DateTime.Now.AddDays(int.Parse(chosenPostMode.Duration));
            chosenPost.PostModeId = postModeId;

            await _productPostRepository.Update(chosenPost);
        }

        public async Task<List<PaymentResponseModel>> GetPostPaymentRecords(int? pageIndex, string postId)
        {
            var list = await _paymentRepository.Get(filter: p => p.ProductPostId.Equals(postId),
                                                                   orderBy: p => p.OrderByDescending(p => p.PaymentDate),
                                                                   pageIndex: pageIndex ?? 1,
                                                                   pageSize: ItemPerPage,
                                                                   includeProperties: "PostMode"
                                                                   );
            var paymentRecords = _mapper.Map<List<PaymentResponseModel>>(list);
            return paymentRecords;
        }
    }
}
