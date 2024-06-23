using BusinessObjects.DTOs.CampusDTOs;
using GoodsExchangeAtFUManagement.Service.Services.CampusServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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

        //private string GetTokenFromHeader()
        //{
        //    var authHeader = Request.Headers["Authorization"].ToString();
        //    if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
        //    {
        //        throw new UnauthorizedAccessException("Authorization token is missing or invalid.");
        //    }

        //    return authHeader.Split(" ")[1];
        //}


        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateCampus([FromBody] CampusCreateRequestModel request)
        {
            await _campusService.CreateCampus(request);
            return Ok("Campus created successfully!");
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
