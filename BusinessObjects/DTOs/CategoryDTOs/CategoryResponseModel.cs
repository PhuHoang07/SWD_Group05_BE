using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTOs.CategoryDTOs
{
    public class CategoryResponseModel
    {
        public string Id { get; set; } = null!;

        public string Name { get; set; } = null!;

        public bool Status { get; set; }
    }
}
