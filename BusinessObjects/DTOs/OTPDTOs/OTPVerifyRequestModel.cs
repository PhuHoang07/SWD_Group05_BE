using BusinessObjects.CustomDataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodsExchangeAtFUManagement.Repository.DTOs.OTPDTOs
{
    public class OTPVerifyRequestModel
    {
        [FPTEmail]
        [Required(ErrorMessage = "Please input your email")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Please input your OTP")]
        public string OTP { get; set; } = null!;
    }
}
