using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTOs.CategoryDTOs
{
    public class CategoryRequestModel
    {
        [Required(ErrorMessage = "Please input campus id")]
        public string Id { get; set; } = null!;

        [Required(ErrorMessage = "Please input campus name")]
        public string Name { get; set; } = null!;
    }
}
