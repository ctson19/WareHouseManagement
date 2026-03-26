using System;
using System.Collections.Generic;

namespace WareManagement.Models;

public partial class Warehouse
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public string? Name { get; set; }

    public string? Address { get; set; }

    public bool? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<ExportReceipt> ExportReceipts { get; set; } = new List<ExportReceipt>();

    public virtual ICollection<ImportReceipt> ImportReceipts { get; set; } = new List<ImportReceipt>();

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<Location> Locations { get; set; } = new List<Location>();

    public virtual ICollection<StockCount> StockCounts { get; set; } = new List<StockCount>();

    public virtual ICollection<StockTransaction> StockTransactions { get; set; } = new List<StockTransaction>();

    public virtual ICollection<Transfer> TransferFromWarehouses { get; set; } = new List<Transfer>();

    public virtual ICollection<Transfer> TransferToWarehouses { get; set; } = new List<Transfer>();

    public virtual User? UpdatedByNavigation { get; set; }
}
