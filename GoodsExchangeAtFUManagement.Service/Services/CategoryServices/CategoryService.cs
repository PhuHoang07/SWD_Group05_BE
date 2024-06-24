using AutoMapper;
using BusinessObjects.DTOs.CampusDTOs;
using BusinessObjects.DTOs.CategoryDTOs;
using BusinessObjects.Models;
using GoodsExchangeAtFUManagement.Repository.Repositories.CategoryRepositories;
using GoodsExchangeAtFUManagement.Service.Ultis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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


        public async Task CreateCategory(CategoryCreateRequestModel request)
        {
            Category currentCategory = await _categoryRepository.GetSingle(c => c.Name.Equals(request.Name));
            if (currentCategory != null)
            {
                throw new CustomException("Category Name has existed");
            }
            Category newCategory = _mapper.Map<Category>(request);
            newCategory.Id = Guid.NewGuid().ToString();
            newCategory.Status = true;
            await _categoryRepository.Insert(newCategory);
        }

        public async Task DeleteCategory(string id)
        {
            Category deleteCategory = await _categoryRepository.GetSingle(c => c.Id.Equals(id));
            if (deleteCategory == null)
            {
                throw new CustomException("Category not found");
            }
            if (deleteCategory.Status == false)
            {
                throw new CustomException("The category is already be removed");
            }
            deleteCategory.Status = false;
            await _categoryRepository.Update(deleteCategory);
        }

        public async Task<List<CategoryResponseModel>> GetAllCategory(Expression<Func<Category, bool>> filter, int pageIndex, int pageSize)
        {
            var categories = await _categoryRepository.Get(c => c.Status == true, pageIndex: pageIndex, pageSize: pageSize);
            var categoryResponses = _mapper.Map<List<CategoryResponseModel>>(categories);
            return categoryResponses;
        }

        public async Task<CategoryResponseModel> GetCategoryById(string id)
        {
            var category = await _categoryRepository.GetSingle(c => c.Id.Equals(id));
            if (category == null)
            {
                throw new CustomException("Category not found");
            }
            var categoryResponse = _mapper.Map<CategoryResponseModel>(category);
            return categoryResponse;
        }

        public async Task UpdateCategory(CategoryRequestModel request)
        {
            var category = await _categoryRepository.GetSingle(c => c.Id.Equals(request.Id));
            if (category == null || category.Status == false)
            {
                throw new CustomException("Category not found");
            }
            category.Name = request.Name;
            await _categoryRepository.Update(category);
        }
    }
}
