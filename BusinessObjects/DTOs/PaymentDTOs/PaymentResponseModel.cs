using BusinessObjects.DTOs.PostModeDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTOs.PaymentDTOs
{
    public class PaymentResponseModel
    {
        public string Id { get; set; } = null!;
        public DateTime PaymentDate { get; set; }
        public string Price { get; set; } = null!;
        public virtual PostModeResponseModel PostMode { get; set; } = null!;

    }
}
