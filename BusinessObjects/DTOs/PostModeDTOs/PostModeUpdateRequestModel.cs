using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTOs.PostModeDTOs
{
    public class PostModeUpdateRequestModel
    {
        [Required(ErrorMessage = "Please input type name for post mode")]
        public string Type { get; set; } = null!;

        [Required(ErrorMessage = "Please input duration(number of days) for post mode")]
        [RegularExpression("([0-9]+)", ErrorMessage = "Please input a number")]
        public string Duration { get; set; } = null!;

        [Required(ErrorMessage = "Please input price(how many coins) for post mode")]
        [RegularExpression("([0-9]+)", ErrorMessage = "Please input a number")]
        public string Price { get; set; } = null!;

        [Required(ErrorMessage = "Please choose status of post mode")]
        public bool Status { get; set; }

    }
}
