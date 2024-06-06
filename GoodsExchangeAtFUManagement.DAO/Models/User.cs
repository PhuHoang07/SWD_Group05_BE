﻿using System;
using System.Collections.Generic;

namespace GoodsExchangeAtFUManagement.DAO.Models;

public partial class User
{
    public string Id { get; set; } = null!;

    public string Fullname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public byte[] Password { get; set; } = null!;

    public byte[] Salt { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Role { get; set; } = null!;

    public string Status { get; set; } = null!;

    public int Balance { get; set; }

    public virtual ICollection<Chat> Chats { get; set; } = new List<Chat>();

    public virtual ICollection<CoinTransaction> CoinTransactions { get; set; } = new List<CoinTransaction>();

    public virtual ICollection<Otpcode> Otpcodes { get; set; } = new List<Otpcode>();

    public virtual ICollection<ProductPost> ProductPosts { get; set; } = new List<ProductPost>();

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();
}
