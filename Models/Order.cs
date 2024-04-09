using System;
using System.Collections.Generic;

namespace backendaspnet.Models;

public partial class Order
{
    public int Id { get; set; }

    public string? Address { get; set; }

    public string? Email { get; set; }

    public string? Fullname { get; set; }

    public string? Note { get; set; }

    public DateTime? OrderDate { get; set; }

    public string? PhoneNumber { get; set; }

    public int? Status { get; set; }

    public int? TotalMoney { get; set; }

    public int? UserId { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual User? User { get; set; }
}
