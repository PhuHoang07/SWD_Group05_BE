using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class ProductTransaction
{
    public string Id { get; set; } = null!;

    public string Price { get; set; } = null!;

    public DateTime TransactAt { get; set; }

    public string CampusId { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string ProductPostId { get; set; } = null!;

    public string BuyerId { get; set; } = null!;

    public virtual Campus Campus { get; set; } = null!;

    public virtual ProductPost ProductPost { get; set; } = null!;
}
