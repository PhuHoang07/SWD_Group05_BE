using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTOs.VnPayDTOs
{
    public class VnPaymentRequestModel
    {
        public string OrderId { get; set; }
        public string PaymentId { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedDate { get; set; }
        public string RedirectUrl { get; set; }
    }
}
