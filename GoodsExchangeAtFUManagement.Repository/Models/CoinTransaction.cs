using System;
using System.Collections.Generic;

namespace GoodsExchangeAtFUManagement.Repository.Models;

public partial class CoinTransaction
{
    public string Id { get; set; } = null!;

    public DateTime TransactAt { get; set; }

    public string Status { get; set; } = null!;

    public string CoinPackId { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public virtual CoinPack CoinPack { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
