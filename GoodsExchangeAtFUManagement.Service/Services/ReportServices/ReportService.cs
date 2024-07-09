using AutoMapper;
using BusinessObjects.DTOs.ProductPostDTOs;
using BusinessObjects.DTOs.ReportDTOs;
using BusinessObjects.Models;
using GoodsExchangeAtFUManagement.Repository.Repositories.ReportRepositories;
using GoodsExchangeAtFUManagement.Service.Ultis;
using MailKit.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public async Task UpdateReport(ReportRequestModel request)
        {
            var report = await _reportRepository.GetSingle(r => r.Id.Equals(request.Id));
            if (report == null)
            {
                throw new CustomException("Report not found");
            }
 
            if (!string.IsNullOrEmpty(request.Content))
            {
                report.Content = request.Content;
            }

            await _reportRepository.Update(report);
        }


        public async Task<List<ReportResponseModel>> ViewAllReports(DateTime? searchDate, string createdBy, int pageIndex, int pageSize)
        {
           
            Expression<Func<Report, bool>> filter = r => true;
            if (searchDate.HasValue)
            {
                filter = filter.And(r => r.Date.Date == searchDate.Value.Date);
            }

            if (!string.IsNullOrEmpty(createdBy))
            {
                filter = filter.And(r => r.CreatedBy.ToLower().Contains(createdBy.ToLower()));
            }

            Func<IQueryable<Report>, IOrderedQueryable<Report>> orderBy = q => q.OrderByDescending(r => r.Date);
        
            var reports = await _reportRepository.Get(filter, orderBy, includeProperties: "CreatedByNavigation,ProductPost", pageIndex, pageSize);
         
            var reportResponses = _mapper.Map<List<ReportResponseModel>>(reports);

            return reportResponses;
        }


    }
}
