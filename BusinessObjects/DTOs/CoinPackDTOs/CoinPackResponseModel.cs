using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTOs.CoinPackDTOs
{
    public class CoinPackResponseModel
    {
        public string Id { get; set; } = null!;

        public int CoinAmount { get; set; }

        public string Price { get; set; } = null!;
    }
}
