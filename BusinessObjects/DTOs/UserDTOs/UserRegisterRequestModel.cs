using BusinessObjects.CustomDataAnnotations;
using GoodsExchangeAtFUManagement.Repository.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodsExchangeAtFUManagement.Repository.DTOs.UserDTOs
{
    public class UserRegisterRequestModel
    {
        [Required(ErrorMessage = "Please input full name")]
        public string Fullname { get; set; } = null!;

        [Required(ErrorMessage = "Please input email")]
        [FPTEmail]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Please input phone number")]
        [Phone]
        public string PhoneNumber { get; set; } = null!;

    }

    public class UserRegisterRequestTestingModel
    {
        [Required(ErrorMessage = "Please input full name")]
        public string Fullname { get; set; } = null!;

        [Required(ErrorMessage = "Please input email")]
        [FPTEmail]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Please input password")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Please input role")]
        [EnumDataType(typeof(RoleEnums), ErrorMessage = "Invalid role.")]
        public string Role { get; set; } = null!;

        [Required(ErrorMessage = "Please input phone number")]
        [Phone]
        public string PhoneNumber { get; set; } = null!;
    }
}
