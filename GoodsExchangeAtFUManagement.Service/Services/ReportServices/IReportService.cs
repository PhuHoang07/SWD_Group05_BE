﻿using BusinessObjects.DTOs.CampusDTOs;
using BusinessObjects.DTOs.ProductPostDTOs;
using BusinessObjects.DTOs.ReportDTOs;
using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodsExchangeAtFUManagement.Service.Services.ReportServices
{
    public interface IReportService
    {
        Task CreateReport(CreateReportRequestModel request, string token);
        Task<List<ReportResponseModel>> ViewAllReports(DateTime? searchDate, int? pageIndex, int pageSize);
        Task UpdateReport(ReportRequestModel request);
        Task<Report> ChangeReportStatus(string id, string status);
    }
}
