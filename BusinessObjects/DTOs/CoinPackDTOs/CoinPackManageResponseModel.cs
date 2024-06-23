using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTOs.CoinPackDTOs
{
    public class CoinPackManageResponseModel
    {
        public string Id { get; set; } = null!;

        public int CoinAmount { get; set; }

        public string Price { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public string Status { get; set; } = null!;
    }
}
