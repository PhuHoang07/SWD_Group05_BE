using BusinessObjects.DTOs.CampusDTOs;
using BusinessObjects.DTOs.ReportDTOs;
using GoodsExchangeAtFUManagement.Service.Services.CampusServices;
using GoodsExchangeAtFUManagement.Service.Services.ReportServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoodsExchangeAtFUManagement.Controllers
{
    [Route("api/report")]
    [ApiController]
    [Authorize]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateReport([FromBody] CreateReportRequestModel request)
        {
            await _reportService.CreateReport(request);
            return Ok("Report created successfully!");
        }
    }
}
