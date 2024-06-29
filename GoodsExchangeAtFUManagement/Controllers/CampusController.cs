using BusinessObjects.DTOs.CampusDTOs;
using GoodsExchangeAtFUManagement.Service.Services.CampusServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoodsExchangeAtFUManagement.Controllers
{
    [Route("api/campus")]
    [ApiController]
    [Authorize]
    public class CampusController : ControllerBase
    {
        private readonly ICampusService _campusService;

        public CampusController(ICampusService campusService)
        {
            _campusService = campusService;
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateCampus([FromBody] CampusCreateRequestModel request)
        {
            await _campusService.CreateCampus(request);
            return Ok("Campus created successfully!");
        }

        [HttpGet]
        [Route("view-all")]
        public async Task<IActionResult> GetAllCampus( int pageIndex, int pageSize, string searchQuery = null)
        {
            var campuses = await _campusService.GetAllCampus(searchQuery, pageIndex, pageSize);
            return Ok(campuses);
        }

        [HttpGet]
        [Route("view/{id}")]
        public async Task<IActionResult> GetCampusById(string id)
        {
            var campus = await _campusService.GetCampusById(id);
            return Ok(campus);
        }

        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> UpdateCampus([FromBody] CampusRequestModel request)
        {

            await _campusService.UpdateCampus(request);
            return Ok("Campus updated successfully!");
        }

        [HttpPut]
        [Route("soft-remove")]
        public async Task<IActionResult> SoftRemoveCampus(string id)
        {
            await _campusService.DeleteCampus(id);
            return Ok("Campus deleted successfully!");
        }
    }
}
