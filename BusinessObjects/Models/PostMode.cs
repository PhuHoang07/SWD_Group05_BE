using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class PostMode
{
    public string Id { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string Duration { get; set; } = null!;

    public string Price { get; set; } = null!;

    public bool Status { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<ProductPost> ProductPosts { get; set; } = new List<ProductPost>();
}
