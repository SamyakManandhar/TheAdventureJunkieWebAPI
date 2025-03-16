using System;
using System.Collections.Generic;

namespace TheAdventureJunkieWebAPI.Models;

public partial class ShoppingCartItem
{
    public int ShoppingCartItemId { get; set; }

    public int EventId { get; set; }

    public int Amount { get; set; }

    public string? ShoppingCartId { get; set; }

    public virtual Event Event { get; set; } = null!;
}
