using AutoMapper;
using BusinessObjects.DTOs.ProductPostDTOs;
using BusinessObjects.DTOs.ReportDTOs;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using GoodsExchangeAtFUManagement.Repository.Repositories.ReportRepositories;
using GoodsExchangeAtFUManagement.Repository.Repositories.UserRepositories;
using GoodsExchangeAtFUManagement.Service.Ultis;
using MailKit.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.WebSockets;
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
            var user = await _userRepository.GetSingle(u => u.Id == userId);
            if (user == null)
            {
                throw new CustomException("User not found.");
            }
            var currUserId = user.Id;

            Report currentReport = await _reportRepository.GetSingle(r => r.ProductPostId == request.ProductPostId && r.CreatedBy == currUserId);
            if (currentReport != null)
            {
                throw new CustomException("A report for this product post by this user already exists.");
            }
            Report newReport = _mapper.Map<Report>(request);
            newReport.Id = Guid.NewGuid().ToString();
            newReport.CreatedBy = userId;
            newReport.Date = DateTime.UtcNow;
            newReport.Status = ReportStatus.Pending.ToString();
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


        public async Task<List<ReportResponseModel>> ViewAllReports(DateTime? searchDate, int? pageIndex, int pageSize)
        {

            var reportResponses = new List<ReportResponseModel>();
            Expression<Func<Report, bool>> filter = r => true;
            if (searchDate.HasValue)
            {
                filter = filter.And(r => r.Date.Date == searchDate.Value.Date && r.Status.Equals(ReportStatus.Pending.ToString()));
            }

            Func<IQueryable<Report>, IOrderedQueryable<Report>> orderBy = q => q.OrderByDescending(r => r.Date);

            var reports = await _reportRepository.Get(filter, orderBy, pageIndex: pageIndex ?? 1, pageSize: pageSize);

            foreach (var report in reports)
            {
                var user = await _userRepository.GetSingle(u => u.Id == report.CreatedBy);
                if (user == null)
                {
                    throw new CustomException($"User with ID '{report.CreatedBy}' not found.");
                }

                reportResponses.Add(new ReportResponseModel
                {
                    Id = report.Id,
                    Content = report.Content,
                    Date = report.Date,
                    ProductPostId = report.ProductPostId,
                    CreatedBy = user.Fullname
                });
            }
            return reportResponses;
        }

        public async Task ChangeReportStatus(string id, string status)
        {
            var report = await _reportRepository.GetSingle(r => r.Id.Equals(id));
            if (report == null)
            {
                throw new CustomException("Report not found");
            }

            if (!report.Status.Equals(ReportStatus.Pending.ToString()))
            {
                throw new CustomException("This report is not in pending status");
            }

            if (!Enum.IsDefined(typeof(ReportStatus), status))
            {
                throw new CustomException("Please input valid report status");
            }

            report.Status = status;
            await _reportRepository.Update(report);
        }
    }
}
