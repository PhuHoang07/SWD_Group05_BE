﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTOs.ProductPostDTOs
{
    public class ProductPostUpdateRequestModel
    {
        [Required(ErrorMessage = "Please enter title of product post")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Please enter description of product post")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Please enter price of product in post")]
        public string Price { get; set; } = null!;

        [Required(ErrorMessage = "Please choose category of product post")]
        public string CategoryId { get; set; } = null!;

        [Required(ErrorMessage = "Please choose campus of product post")]
        public string CampusId { get; set; } = null!;

        [Required(ErrorMessage = "Please choose image(s) for product post")]
        public List<string> ImagesUrl { get; set; } = null!;
    }
}
