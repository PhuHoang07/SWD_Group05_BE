using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Campus
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public bool Status { get; set; }

    public virtual ICollection<ProductPost> ProductPosts { get; set; } = new List<ProductPost>();
}
