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
        public async Task<IActionResult> SendOTP([FromBody] string email)
        {
            try
            {
                var otp = await _otpService.CreateOTPCodeForEmail(email);
                return Ok(otp);
            }
            catch (CustomException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
