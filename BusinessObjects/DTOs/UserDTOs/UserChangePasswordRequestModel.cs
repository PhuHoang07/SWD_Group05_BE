﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodsExchangeAtFUManagement.Repository.DTOs.UserDTOs
{
    public class UserChangePasswordRequestModel
    {
        [Required(ErrorMessage ="Please input old password")]
        public string OldPassword { get; set; } = null!;
        
        [Required(ErrorMessage = "Please input new password")]
        public string NewPassword { get; set; } = null!;
        
        [Required(ErrorMessage = "Please input confirm password")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
