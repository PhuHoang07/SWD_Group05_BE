﻿using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Payment
{
    public string Id { get; set; } = null!;

    public DateTime PaymentDate { get; set; }

    public string Price { get; set; } = null!;

    public string ProductPostId { get; set; } = null!;

    public string PostModeId { get; set; } = null!;

    public virtual PostMode PostMode { get; set; } = null!;

    public virtual ProductPost ProductPost { get; set; } = null!;
}
