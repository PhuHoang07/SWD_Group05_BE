using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTOs.CoinTransactionDTOs
{
    public class CoinTransactionPaymentResponseModel
    {
        public string transactId { get; set; } = null!;
        public string transactUrl { get; set; } = null!;
    }
}
