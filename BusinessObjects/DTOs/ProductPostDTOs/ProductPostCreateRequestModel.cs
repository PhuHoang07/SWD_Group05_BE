using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTOs.ProductPostDTOs
{
    public class ProductPostCreateRequestModel
    {
        [Required(ErrorMessage = "Please enter title of product post")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Please enter description of product post")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Please enter price of product in post")]
        public string Price { get; set; } = null!;

        [Required(ErrorMessage = "Please choose category of product post")]
        public string CategoryId { get; set; } = null!;

        [Required(ErrorMessage = "Please choose post mode of product post")]
        public string PostModeId { get; set; } = null!;
    }
}
