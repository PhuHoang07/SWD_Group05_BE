﻿using System;
using System.Collections.Generic;

namespace GoodsExchangeAtFUManagement.Repository.Models;

public partial class Category
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public virtual ICollection<ProductPost> ProductPosts { get; set; } = new List<ProductPost>();
}
