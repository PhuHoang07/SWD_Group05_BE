using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTOs.ProductPostDTOs
{
    public class WaitingProductPostResponseModel
    {
        public string Id { get; set; } = null!;

        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string Price { get; set; } = null!;

        public DateTime ExpiredDate { get; set; }

        public string CreatedBy { get; set; } = null!;

        public string CategoryId { get; set; } = null!;

        public string PostModeId { get; set; } = null!;
    }
}
