using BusinessObjects.DTOs.CampusDTOs;
using BusinessObjects.DTOs.CategoryDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodsExchangeAtFUManagement.Service.Services.CategoryServices
{
    public interface ICategoryService
    {
        Task<List<CategoryResponseModel>> GetAllCategory();
        Task<CategoryResponseModel> GetCategoryById(string id);
        Task CreateCategory(CategoryCreateRequestModel request);
        Task UpdateCategory(CategoryRequestModel request);
        Task DeleteCategory(string id);
    }
}
