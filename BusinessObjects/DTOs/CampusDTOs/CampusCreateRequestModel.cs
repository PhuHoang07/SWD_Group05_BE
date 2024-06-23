using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTOs.CampusDTOs
{
    public class CampusCreateRequestModel
    {
        [Required(ErrorMessage = "Please input campus name")]
        public string Name { get; set; } = null!;
    }
}
