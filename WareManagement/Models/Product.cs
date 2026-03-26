using System;
using System.Collections.Generic;

namespace WareManagement.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public string? Name { get; set; }

    public int? UnitId { get; set; }

    public string? Barcode { get; set; }

    public decimal? ImportPrice { get; set; }

    public decimal? SalePrice { get; set; }

    public decimal? MinStock { get; set; }

    public decimal? MaxStock { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<ExportReceiptDetail> ExportReceiptDetails { get; set; } = new List<ExportReceiptDetail>();

    public virtual ICollection<ImportReceiptDetail> ImportReceiptDetails { get; set; } = new List<ImportReceiptDetail>();

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<StockCountDetail> StockCountDetails { get; set; } = new List<StockCountDetail>();

    public virtual ICollection<StockTransaction> StockTransactions { get; set; } = new List<StockTransaction>();

    public virtual ICollection<TransferDetail> TransferDetails { get; set; } = new List<TransferDetail>();

    public virtual Unit? Unit { get; set; }

    public virtual User? UpdatedByNavigation { get; set; }
}
