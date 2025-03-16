using System;
using System.Collections.Generic;

namespace TheAdventureJunkieWebAPI.Models;

public partial class Event
{
    public int EventId { get; set; }

    public string Name { get; set; } = null!;

    public string? ShortDescription { get; set; }

    public string? LongDescription { get; set; }

    public decimal Price { get; set; }

    public string? ImageUrl { get; set; }

    public string? EventLocation { get; set; }

    public DateTime EventDateTime { get; set; }

    public int CategoryId { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<ShoppingCartItem> ShoppingCartItems { get; set; } = new List<ShoppingCartItem>();
}
