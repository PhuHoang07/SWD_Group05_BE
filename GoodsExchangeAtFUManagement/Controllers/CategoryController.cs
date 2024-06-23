using BusinessObjects.DTOs.CampusDTOs;
using BusinessObjects.DTOs.CategoryDTOs;
using BusinessObjects.Models;
using GoodsExchangeAtFUManagement.Service.Services.CampusServices;
using GoodsExchangeAtFUManagement.Service.Services.CategoryServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GoodsExchangeAtFUManagement.Controllers
{
    [Route("api/category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("create")]
        public async Task<IActionResult> CreateCategory(CategoryCreateRequestModel request)
        {
            await _categoryService.CreateCategory(request);
            return Ok("Category created successfully!");
        }

        [HttpGet]
        [Route("view-all")]
        public async Task<IActionResult> GetAllCategory(int pageIndex, int pageSize)
        {
            Expression<Func<Category, bool>> filter = c => c.Status == true;
            var categoryList = await _categoryService.GetAllCategory(filter, pageIndex, pageSize);
            return Ok(categoryList);
        }

        [HttpGet]
        [Route("view/{id}")]
        public async Task<IActionResult> GetCategoryById(string id)
        {
            var category = await _categoryService.GetCategoryById(id);
            return Ok(category);
        }

        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> UpdateCategory(CategoryRequestModel request)
        {
            await _categoryService.UpdateCategory(request);
            return Ok("Category updated successfully!");
        }

        [HttpPut]
        [Route("soft-remove")]
        public async Task<IActionResult> SoftRemovedCategory(string id)
        {
            await _categoryService.DeleteCategory(id);
            return Ok("Category soft removed successfully!");
        }
    }
}
