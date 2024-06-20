using BusinessObjects.DTOs.CampusDTOs;
using BusinessObjects.DTOs.CategoryDTOs;
using GoodsExchangeAtFUManagement.Service.Services.CampusServices;
using GoodsExchangeAtFUManagement.Service.Services.CategoryServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GoodsExchangeAtFUManagement.Controllers
{
    [Route("api/category")]
    [ApiController]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
           _categoryService = categoryService;
        }

        private string GetTokenFromHeader()
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                throw new UnauthorizedAccessException("Authorization token is missing or invalid.");
            }

            return authHeader.Split(" ")[1];
        }


        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryCreateRequestModel request)
        {
            if (request == null)
            {
                return BadRequest("Invalid category data.");
            }

            try
            {
                string token = GetTokenFromHeader();
                await _categoryService.CreateCategory(request, token);
                return Ok("Category created successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("view-all")]
        public async Task<IActionResult> GetAllCategory()
        {
            try
            {
                var campuses = await _categoryService.GetAllCategory();
                return Ok(campuses);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("view/{id}")]
        public async Task<IActionResult> GetCategoryById(string id)
        {
            try
            {

                var campus = await _categoryService.GetCategoryById(id);
                if (campus == null)
                {
                    return NotFound("Category not found.");
                }
                return Ok(campus);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> UpdateCategory([FromBody] CategoryRequestModel request)
        {
            if (request == null)
            {
                return BadRequest("Invalid category data.");
            }

            try
            {
                string token = GetTokenFromHeader();
                await _categoryService.UpdateCategory(request, token);
                return Ok("Category updated successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> DeleteCategory(string id)
        {
            try
            {
                string token = GetTokenFromHeader();
                await _categoryService.DeleteCategory(id, token);
                return Ok("Category deleted successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
