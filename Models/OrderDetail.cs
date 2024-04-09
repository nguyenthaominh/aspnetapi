using System;
using System.Collections.Generic;

namespace backendaspnet.Models;

public partial class OrderDetail
{
    public int Id { get; set; }

    public int? Num { get; set; }

    public int? Price { get; set; }

    public int? TotalMoney { get; set; }

    public int? OrderId { get; set; }

    public int? ProductId { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Product? Product { get; set; }
}
