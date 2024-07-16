using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTOs.CoinTransactionDTOs
{
    public class CoinTransactionCreateReqModel
    {
        public string coinPackId { get; set; } = null!;
        public string redirectUrl { get; set; } = null!;
    }
}
