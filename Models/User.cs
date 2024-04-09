using System;
using System.Collections.Generic;

namespace backendaspnet.Models;

public partial class User
{
    public int Id { get; set; }

    public string? Address { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? Deleted { get; set; }

    public string? Email { get; set; }

    public string? Fullname { get; set; }

    public string? Password { get; set; }

    public string? PhoneNumber { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? RoleId { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Role? Role { get; set; }
}
