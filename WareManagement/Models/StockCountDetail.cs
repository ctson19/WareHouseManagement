using System;
using System.Collections.Generic;

namespace WareManagement.Models;

public partial class StockCountDetail
{
    public int Id { get; set; }

    public int StockCountId { get; set; }

    public int ProductId { get; set; }

    public int LocationId { get; set; }

    public decimal? SystemQty { get; set; }

    public decimal? ActualQty { get; set; }

    public decimal? DifferenceQty { get; set; }

    public virtual Location Location { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual StockCount StockCount { get; set; } = null!;
}
