using System;
using System.Collections.Generic;

namespace TheAdventureJunkieWebAPI.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string AddressLine1 { get; set; } = null!;

    public string? AddressLine2 { get; set; }

    public string ZipCode { get; set; } = null!;

    public string City { get; set; } = null!;

    public string? State { get; set; }

    public string Country { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Email { get; set; } = null!;

    public decimal OrderTotal { get; set; }

    public DateTime OrderPlaced { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
