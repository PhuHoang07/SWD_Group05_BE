using BusinessObjects.DTOs.CampusDTOs;
using GoodsExchangeAtFUManagement.Service.Services.CampusServices;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GoodsExchangeAtFUManagement.Controllers
{
    [Route("api/campus")]
    [ApiController]
    public class CampusController : ControllerBase
    {
        private readonly ICampusService _campusService;

        public CampusController(ICampusService campusService)
        {
            _campusService = campusService;
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateCampus([FromBody] CampusRequestModel request)
        {
            if (request == null)
            {
                return BadRequest("Invalid campus data.");
            }

            try
            {
                await _campusService.CreateCampus(request);
                return Ok("Campus created successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("view-all")]
        public async Task<IActionResult> GetAllCampus()
        {
            var campuses = await _campusService.GetAllCampus();
            return Ok(campuses);
        }

        [HttpGet]
        [Route("view/{id}")]
        public async Task<IActionResult> GetCampusById(string id)
        {
            try
            {
                var campus = await _campusService.GetCampusById(id);
                if (campus == null)
                {
                    return NotFound("Campus not found.");
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
        public async Task<IActionResult> UpdateCampus([FromBody] CampusRequestModel request)
        {
                await _campusService.UpdateCampus(request);
                return Ok("Campus updated successfully!");
        }

        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> DeleteCampus(string id)
        {
            try
            {
                await _campusService.DeleteCampus(id);
                return Ok("Campus deleted successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
