using System;
using System.Collections.Generic;

namespace WareManagement.Models;

public partial class StockTransaction
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int WarehouseId { get; set; }

    public int LocationId { get; set; }

    public decimal? Quantity { get; set; }

    public string? TransactionType { get; set; }

    public int? ReferenceId { get; set; }

    public string? ReferenceType { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual Location Location { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual Warehouse Warehouse { get; set; } = null!;
}
