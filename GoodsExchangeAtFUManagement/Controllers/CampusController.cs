﻿using BusinessObjects.DTOs.CampusDTOs;
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
        public async Task<IActionResult> CreateCampus([FromBody] CampusCreateRequestModel request)
        {
            if (request == null)
            {
                return BadRequest("Invalid campus data.");
            }

            try
            { 
                string token = GetTokenFromHeader();
                await _campusService.CreateCampus(request, token);
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
            try
            {
                var campuses = await _campusService.GetAllCampus();
                return Ok(campuses);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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
            if (request == null)
            {
                return BadRequest("Invalid campus data.");
            }

            try
            {
                string token = GetTokenFromHeader();
                await _campusService.UpdateCampus(request,token);
                return Ok("Campus updated successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> DeleteCampus(string id)
        {
            try
            {
                string token = GetTokenFromHeader();
                await _campusService.DeleteCampus(id, token);
                return Ok("Campus deleted successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
