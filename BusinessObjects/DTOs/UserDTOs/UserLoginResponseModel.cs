﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodsExchangeAtFUManagement.Repository.DTOs.UserDTOs
{
    public class UserLoginResponseModel
    {
        public UserInfo UserInfo { get; set; } = null!;
        public string token { get; set; } = null!;
        public string refreshToken { get; set; } = null!;
    }

    public class UserInfo
    {
        public string Id { get; set; } = null!;
        public string Fullname { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public int Balance { get; set; }
    }
}
