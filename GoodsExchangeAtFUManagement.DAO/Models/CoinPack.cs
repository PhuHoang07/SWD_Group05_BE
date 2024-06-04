using System;
using System.Collections.Generic;

namespace GoodsExchangeAtFUManagement.DAO.Models;

public partial class CoinPack
{
    public string Id { get; set; } = null!;

    public int CoinAmount { get; set; }

    public string Price { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<CoinTransaction> CoinTransactions { get; set; } = new List<CoinTransaction>();
}
