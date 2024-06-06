using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodsExchangeAtFUManagement.Repository.DTOs.OTPDTOs
{
    public class OTPVerifyRequestModel
    {
        public string Email { get; set; } = null!;

        public string OTP { get; set; } = null!;
    }
}
