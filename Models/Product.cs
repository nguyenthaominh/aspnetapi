using System;
using System.Collections.Generic;

namespace backendaspnet.Models;

public partial class Product
{
    public int Id { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? Deleted { get; set; }

    public string? Description { get; set; }

    public int? Discount { get; set; }

    public int? Price { get; set; }

    public string? Thumbnail { get; set; }

    public string? Title { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CategoryId { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<Gallery> Galleries { get; set; } = new List<Gallery>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
