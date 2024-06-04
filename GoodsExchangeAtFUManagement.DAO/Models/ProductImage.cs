using System;
using System.Collections.Generic;

namespace GoodsExchangeAtFUManagement.DAO.Models;

public partial class ProductImage
{
    public string Id { get; set; } = null!;

    public string Url { get; set; } = null!;

    public string ProductPostId { get; set; } = null!;

    public virtual ProductPost ProductPost { get; set; } = null!;
}
