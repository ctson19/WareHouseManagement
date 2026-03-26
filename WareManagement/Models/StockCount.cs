using System;
using System.Collections.Generic;

namespace WareManagement.Models;

public partial class StockCount
{
    public int Id { get; set; }

    public int WarehouseId { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<StockCountDetail> StockCountDetails { get; set; } = new List<StockCountDetail>();

    public virtual Warehouse Warehouse { get; set; } = null!;
}
