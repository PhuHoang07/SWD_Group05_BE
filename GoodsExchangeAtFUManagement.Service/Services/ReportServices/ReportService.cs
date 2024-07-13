using AutoMapper;
using BusinessObjects.DTOs.ProductPostDTOs;
using BusinessObjects.DTOs.ReportDTOs;
using BusinessObjects.Models;
using GoodsExchangeAtFUManagement.Repository.Repositories.ReportRepositories;
using GoodsExchangeAtFUManagement.Repository.Repositories.UserRepositories;
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
        private readonly IUserRepository _userRepository;

        public ReportService(IReportRepository reportRepository, IMapper mapper, IUserRepository userRepository)
        {
            _mapper = mapper;
            _reportRepository = reportRepository;
            _userRepository = userRepository;
        }

        public async Task CreateReport(CreateReportRequestModel request, string token)
        {
            var userId = JwtGenerator.DecodeToken(token, "userId");
            Report currentReport = await _reportRepository.GetSingle(r => r.ProductPostId == request.ProductPostId && r.CreatedBy == userId);
            if (string.IsNullOrEmpty(request.CreatedBy))
            {
                throw new CustomException("The createdBy field is required.");
            }
            if (currentReport != null)
            {
                throw new CustomException("A report for this product post by this user already exists.");
            }       
            Report newReport = _mapper.Map<Report>(request);
            newReport.Id = Guid.NewGuid().ToString();
            newReport.CreatedBy = userId;
            newReport.Date = DateTime.UtcNow;
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


        public async Task<List<ReportResponseModel>> ViewAllReports(DateTime? searchDate, int pageIndex, int pageSize)
        {
           
            Expression<Func<Report, bool>> filter = r => true;
            if (searchDate.HasValue)
            {
                filter = filter.And(r => r.Date.Date == searchDate.Value.Date);
            }          

            Func<IQueryable<Report>, IOrderedQueryable<Report>> orderBy = q => q.OrderByDescending(r => r.Date);
        
            var reports = await _reportRepository.Get(filter, orderBy, pageIndex: pageIndex, pageSize: pageSize);
         
            var reportResponses = _mapper.Map<List<ReportResponseModel>>(reports);

            return reportResponses;
        }

    }
}
