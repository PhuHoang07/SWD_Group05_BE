using GoodsExchangeAtFUManagement.Repository.DTOs.OTPDTOs;
using GoodsExchangeAtFUManagement.Repository.Models;
using GoodsExchangeAtFUManagement.Repository.UnitOfWork;
using GoodsExchangeAtFUManagement.Service.Services.EmailServices;
using GoodsExchangeAtFUManagement.Service.Ultis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GoodsExchangeAtFUManagement.Service.Services.OTPServices
{
    public class OTPService : IOTPService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;

        public OTPService(IUnitOfWork unitOfWork, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        private string CreateNewOTPCode()
        {
            return new Random().Next(0, 999999).ToString("D6");
        }

        public async Task<string> CreateOTPCodeForEmail(OTPSendEmailRequestModel model)
        {
            User currentUser = await _unitOfWork.UserRepository.GetSingle(u => u.Email.Equals(model.Email));

            if (currentUser == null)
            {
                throw new CustomException("There is no account with this email");
            }

            var latestOTP = await _unitOfWork.OTPCodeRepository.GetSingle(o => o.CreatedBy == currentUser.Id, o => o.OrderByDescending(o => o.CreatedAt));

            if (latestOTP != null)
            {
                if ((DateTime.Now - latestOTP.CreatedAt).TotalMinutes < 2)
                {
                    throw new CustomException($"Cannot send new OTP right now, please wait for {(120 - (DateTime.Now - latestOTP.CreatedAt).TotalSeconds).ToString("0.00")} second(s)");
                }
            }

            string newOTP = CreateNewOTPCode();
            var htmlBody = $"<h1>Your OTP code is:</h1><br/><p>{newOTP}<br/>It will be expired in 30 minutes</p>";
            bool sendEmailSuccess = await _emailService.SendEmail(model.Email, model.Subject, htmlBody);
            if (!sendEmailSuccess)
            {
                throw new CustomException("Error in sending email");
            }
            Otpcode newOTPCode = new Otpcode()
            {
                Id = Guid.NewGuid().ToString(),
                Otp = newOTP,
                CreatedBy = currentUser.Id,
                CreatedAt = DateTime.Now,
                IsUsed = false,
            };

            await _unitOfWork.OTPCodeRepository.Insert(newOTPCode);
            var result = await _unitOfWork.SaveChangeAsync();

            if (result < 1)
            {
                throw new CustomException("Internal Server Error");
            }
            return newOTP;
        }
    }
}
