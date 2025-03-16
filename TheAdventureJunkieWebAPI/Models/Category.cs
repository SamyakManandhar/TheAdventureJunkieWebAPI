using System;
using System.Collections.Generic;

namespace TheAdventureJunkieWebAPI.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
