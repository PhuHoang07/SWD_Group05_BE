using BusinessObjects.CustomDataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodsExchangeAtFUManagement.Repository.DTOs.UserDTOs
{
    public class UserLoginRequestModel
    {
        [FPTEmail]
        [Required(ErrorMessage ="Please input your email")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Please input your password")]
        public string Password { get; set; } = null!;
    }
}
