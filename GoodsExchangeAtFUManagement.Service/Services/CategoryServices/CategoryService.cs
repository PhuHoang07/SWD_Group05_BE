using AutoMapper;
using BusinessObjects.DTOs.CampusDTOs;
using BusinessObjects.DTOs.CategoryDTOs;
using BusinessObjects.Models;
using GoodsExchangeAtFUManagement.Repository.Repositories.CategoryRepositories;
using GoodsExchangeAtFUManagement.Service.Ultis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodsExchangeAtFUManagement.Service.Services.CategoryServices
{
    public class CategoryService : ICategoryService
    {
        private readonly IMapper _mapper;
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _mapper = mapper;
            _categoryRepository = categoryRepository;
        }

        private void EnsureAuthorization(string token, bool requireAdmin = false)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new UnauthorizedAccessException("Authorization required");
            }

            var userId = JwtGenerator.DecodeToken(token, "userId");
            if (userId == null)
            {
                throw new UnauthorizedAccessException("Authorization required");
            }

            if (requireAdmin)
            {
                var role = JwtGenerator.DecodeToken(token, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
                if (role != "Admin")
                {
                    throw new UnauthorizedAccessException("Only admins can perform this action");
                }
            }
        }

        public async Task CreateCategory(CategoryCreateRequestModel request, string token)
        {
            EnsureAuthorization(token, true);
            Category currentCategory = await _categoryRepository.GetSingle(c => c.Name.Equals(request.Name));
            if (currentCategory != null)
            {
                throw new Exception("Category Name has existed");
            }
            Category newCategory = _mapper.Map<Category>(request);
            newCategory.Id = Guid.NewGuid().ToString();
            await _categoryRepository.Insert(newCategory);
        }

        public async Task DeleteCategory(string id, string token)
        {
            EnsureAuthorization(token, true);
            Category deleteCategory = await _categoryRepository.GetSingle(c => c.Id.Equals(id));
            if (deleteCategory == null)
            {
                throw new Exception("Category not found");
            }
            await _categoryRepository.Delete(deleteCategory.Id);
        }

        public async Task<List<CategoryResponseModel>> GetAllCategory()
        {
            var categories = await _categoryRepository.Get();
            var categoryResponses = _mapper.Map<List<CategoryResponseModel>>(categories);
            return categoryResponses;
        }

        public async Task<CategoryResponseModel> GetCategoryById(string id)
        {
            var category = await _categoryRepository.GetSingle(c => c.Id.Equals(id));
            if (category == null)
            {
                throw new Exception("Category not found");
            }
            var categoryResponse = _mapper.Map<CategoryResponseModel>(category);
            return categoryResponse;
        }

        public async Task UpdateCategory(CategoryRequestModel request, string token)
        {
            EnsureAuthorization(token, true);
            var category = await _categoryRepository.GetSingle(c => c.Id.Equals(request.Id));
            if (category == null)
            {
                throw new Exception("Category not found");
            }
            category.Name = request.Name;
            await _categoryRepository.Update(category);
        }
    }
}
