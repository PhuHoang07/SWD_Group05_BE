using BusinessObjects.Models;
using GoodsExchangeAtFUManagement.Repository.DTOs.OTPDTOs;
using GoodsExchangeAtFUManagement.Repository.Repositories.OTPCodeRepositories;
using GoodsExchangeAtFUManagement.Repository.Repositories.UserRepositories;
using GoodsExchangeAtFUManagement.Service.Services.EmailServices;
using GoodsExchangeAtFUManagement.Service.Ultis;

namespace GoodsExchangeAtFUManagement.Service.Services.OTPServices
{
    public class OTPService : IOTPService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IOTPCodeRepository _otpCodeRepository;

        public OTPService(IUserRepository userRepository, IEmailService emailService, IOTPCodeRepository oTPCodeRepository)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _otpCodeRepository = oTPCodeRepository;
        }

        private string CreateNewOTPCode()
        {
            return new Random().Next(0, 999999).ToString("D6");
        }

        public async Task CreateOTPCodeForEmail(OTPSendEmailRequestModel model)
        {
            User currentUser = await _userRepository.GetSingle(u => u.Email.Equals(model.Email));

            if (currentUser == null)
            {
                throw new CustomException("There is no account with this email");
            }

            var latestOTP = await _otpCodeRepository.GetSingle(o => o.CreatedBy == currentUser.Id, o => o.OrderByDescending(o => o.CreatedAt));

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

            await _otpCodeRepository.Insert(newOTPCode);
        }

        public async Task VerifyOTP(OTPVerifyRequestModel model)
        {
            User currentUser = await _userRepository.GetSingle(u => u.Email.Equals(model.Email));

            if (currentUser == null)
            {
                throw new CustomException("There is no account with this email");
            }

            var latestOTP = await _otpCodeRepository.GetSingle(o => o.CreatedBy == currentUser.Id, o => o.OrderByDescending(o => o.CreatedAt));

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

                _otpCodeRepository.Update(latestOTP);
               
            }
        }
    }
}
