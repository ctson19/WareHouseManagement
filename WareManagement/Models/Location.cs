using System;
using System.Collections.Generic;

namespace WareManagement.Models;

public partial class Location
{
    public int Id { get; set; }

    public int WarehouseId { get; set; }

    public int? ParentId { get; set; }

    public string? Name { get; set; }

    public string Type { get; set; } = null!;

    public virtual ICollection<ExportReceiptDetail> ExportReceiptDetails { get; set; } = new List<ExportReceiptDetail>();

    public virtual ICollection<ImportReceiptDetail> ImportReceiptDetails { get; set; } = new List<ImportReceiptDetail>();

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<Location> InverseParent { get; set; } = new List<Location>();

    public virtual Location? Parent { get; set; }

    public virtual ICollection<StockCountDetail> StockCountDetails { get; set; } = new List<StockCountDetail>();

    public virtual ICollection<StockTransaction> StockTransactions { get; set; } = new List<StockTransaction>();

    public virtual Warehouse Warehouse { get; set; } = null!;
}
