using System;
using System.Collections.Generic;

namespace backendaspnet.Models;

public partial class Gallery
{
    public int Id { get; set; }

    public string? Thumbnail { get; set; }

    public int? ProductId { get; set; }

    public virtual Product? Product { get; set; }
}
