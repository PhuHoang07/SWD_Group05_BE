using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTOs.PostModeDTOs
{
    public class PostModeResponseModel
    {
        public string Id { get; set; } = null!;

        public string Type { get; set; } = null!;

        public string Duration { get; set; } = null!;

        public string Price { get; set; } = null!;
    }
}
