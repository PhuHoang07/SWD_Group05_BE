using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTOs.ReportDTOs
{
    public class CreateReportRequestModel
    {
        public string Content { get; set; } = null!;

        public string ProductPostId { get; set; } = null!;

       
    }
}
