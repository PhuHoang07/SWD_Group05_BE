using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTOs.UserDTOs
{
    public class UpdateUserRequestModel
    {
        public string Fullname { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;   
    }
}
