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

        [HttpGet]
        [Route("view-all")]
        public async Task<IActionResult> ViewAllReports(DateTime? searchDate, string createdBy, int pageIndex, int pageSize)
        {
            var reports = await _reportService.ViewAllReports(searchDate, createdBy, pageIndex, pageSize);
            return Ok(reports);
        }

        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> UpdateReport([FromBody] ReportRequestModel request)
        {

            await _reportService.UpdateReport(request);
            return Ok("Report updated successfully!");
        }
    }
}
