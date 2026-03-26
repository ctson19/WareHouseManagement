using System;
using System.Collections.Generic;

namespace WareManagement.Models;

public partial class Inventory
{
    public int Id { get; set; }

    public int WarehouseId { get; set; }

    public int ProductId { get; set; }

    public int LocationId { get; set; }

    public decimal? Quantity { get; set; }

    public virtual Location Location { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual Warehouse Warehouse { get; set; } = null!;
}
