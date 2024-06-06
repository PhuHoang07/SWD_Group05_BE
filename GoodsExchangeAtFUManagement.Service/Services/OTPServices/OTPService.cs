using BusinessObjects.Models;
using GoodsExchangeAtFUManagement.Repository.DTOs.OTPDTOs;
using GoodsExchangeAtFUManagement.Repository.UnitOfWork;
using GoodsExchangeAtFUManagement.Service.Services.EmailServices;
using GoodsExchangeAtFUManagement.Service.Ultis;

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

        public async Task CreateOTPCodeForEmail(OTPSendEmailRequestModel model)
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
            var htmlBody = HTMLEmail.SendingOTPEmail(currentUser.Fullname, newOTP, model.Subject.ToLower());
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
        }

        public async Task VerifyOTP(OTPVerifyRequestModel model)
        {
            User currentUser = await _unitOfWork.UserRepository.GetSingle(u => u.Email.Equals(model.Email));

            if (currentUser == null)
            {
                throw new CustomException("There is no account with this email");
            }

            var latestOTP = await _unitOfWork.OTPCodeRepository.GetSingle(o => o.CreatedBy == currentUser.Id, o => o.OrderByDescending(o => o.CreatedAt));

            if (latestOTP != null)
            {
                if ((DateTime.Now - latestOTP.CreatedAt).TotalMinutes > 30 || latestOTP.IsUsed)
                {
                    throw new CustomException("The OTP is already used or expired");
                }

                if (latestOTP.Otp.Equals(model.OTP))
                {
                    latestOTP.IsUsed = true;
                }
                else
                {
                    throw new CustomException("The OTP is incorrect");
                }

                _unitOfWork.OTPCodeRepository.Update(latestOTP);
                var result = await _unitOfWork.SaveChangeAsync();

                if (result < 1)
                {
                    throw new CustomException("Internal Server Error");
                }
            }
        }
    }
}
