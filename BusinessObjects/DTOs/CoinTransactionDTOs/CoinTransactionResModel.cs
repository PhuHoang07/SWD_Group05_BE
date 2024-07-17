using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTOs.CoinTransactionDTOs
{
    public class CoinTransactionResModel
    {
        public string Id { get; set; } = null!;

        public DateTime TransactAt { get; set; }

        public string Status { get; set; } = null!;

        public int CoinAmount { get; set; }

        public string Price { get; set; } = null!;

        public UserInfo User { get; set; } = null!;
    }

    public class MyCoinTransactionResModel
    {
        public string Id { get; set; } = null!;

        public DateTime TransactAt { get; set; }

        public string Status { get; set; } = null!;

        public int CoinAmount { get; set; }

        public string Price { get; set; } = null!;
    }

    public class UserInfo
    {
        public string Id { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
    }
}
