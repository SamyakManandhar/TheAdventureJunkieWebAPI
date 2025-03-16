using System;
using System.Collections.Generic;

namespace TheAdventureJunkieWebAPI.Models;

public partial class OrderDetail
{
    public int OrderDetailId { get; set; }

    public int OrderId { get; set; }

    public int Amount { get; set; }

    public decimal Price { get; set; }

    public int EventId { get; set; }

    public virtual Event Event { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
