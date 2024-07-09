using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTOs.ReportDTOs
{
    public class ReportResponseModel
    {
        public string Id { get; set; } = null!;

        public string Content { get; set; } = null!;

        public DateTime Date { get; set; }

        public string ProductPostId { get; set; } = null!;

        public string CreatedBy { get; set; } = null!;

        public virtual User CreatedByNavigation { get; set; } = null!;

        public virtual ProductPost ProductPost { get; set; } = null!;
    }

    
}
