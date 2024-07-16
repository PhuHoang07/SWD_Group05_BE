using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTOs.CoinTransactionDTOs
{
    public class CoinTransactionUpdateReqModel
    {
        public string TransactId { get; set; } = null!;
        public string Status { get; set; } = null!;
    }
}
