using System;
using System.Collections.Generic;

namespace GoodsExchangeAtFUManagement.DAO.Models;

public partial class Campus
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public virtual ICollection<ProductTransaction> ProductTransactions { get; set; } = new List<ProductTransaction>();
}
