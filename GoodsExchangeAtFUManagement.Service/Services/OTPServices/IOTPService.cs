using GoodsExchangeAtFUManagement.Repository.DTOs.OTPDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodsExchangeAtFUManagement.Service.Services.OTPServices
{
    public interface IOTPService
    {
        Task CreateOTPCodeForEmail(OTPSendEmailRequestModel model);
        Task VerifyOTP(OTPVerifyRequestModel model);
    }
}
