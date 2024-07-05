using BusinessObjects.DTOs.CampusDTOs;
using BusinessObjects.DTOs.CategoryDTOs;
using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GoodsExchangeAtFUManagement.Service.Services.CategoryServices
{
    public interface ICategoryService
    {
        Task<List<CategoryResponseModel>> GetAllCategory(string searchQuery, int pageIndex, int pageSize);
        Task<CategoryResponseModel> GetCategoryById(string id);
        Task CreateCategory(CategoryCreateRequestModel request);
        Task UpdateCategory(CategoryRequestModel request);
        Task DeleteCategory(string id);
    }
}
