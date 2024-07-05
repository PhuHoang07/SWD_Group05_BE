using AutoMapper;
using BusinessObjects.DTOs.ReportDTOs;
using BusinessObjects.Models;
using GoodsExchangeAtFUManagement.Repository.Repositories.ReportRepositories;
using GoodsExchangeAtFUManagement.Service.Ultis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodsExchangeAtFUManagement.Service.Services.ReportServices
{
    public class ReportService : IReportService
    {
        private readonly IMapper _mapper;
        private readonly IReportRepository _reportRepository;

        public ReportService(IReportRepository reportRepository, IMapper mapper)
        {
            _mapper = mapper;
            _reportRepository = reportRepository;
        }

        public async Task CreateReport(CreateReportRequestModel request)
        {
            Report currentReport = await _reportRepository.GetSingle(r => r.ProductPostId == request.ProductPostId && r.CreatedBy == request.CreatedBy);
            if (currentReport != null)
            {
                throw new CustomException("A report for this product post by this user already exists.");
            }
            Report newReport = _mapper.Map<Report>(request);
            newReport.Id = Guid.NewGuid().ToString();
            await _reportRepository.Insert(newReport);
        }
    }
}
