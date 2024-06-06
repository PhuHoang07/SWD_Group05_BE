using GoodsExchangeAtFUManagement.Repository.DTOs.OTPDTOs;
using GoodsExchangeAtFUManagement.Repository.DTOs.UserDTOs;
using GoodsExchangeAtFUManagement.Service.Services.OTPServices;
using GoodsExchangeAtFUManagement.Service.Ultis;
using Microsoft.AspNetCore.Mvc;

namespace GoodsExchangeAtFUManagement.Controllers
{
    [ApiController]
    [Route("api/otp-management")]
    public class OTPController : Controller
    {
        private readonly IOTPService _otpService;

        public OTPController(IOTPService oTPService)
        {
            _otpService = oTPService;
        }

        [HttpPost]
        [Route("send")]
        public async Task<IActionResult> SendOTP(OTPSendEmailRequestModel model)
        {
            var otp = await _otpService.CreateOTPCodeForEmail(model);
            return Ok();
        }
    }
}
