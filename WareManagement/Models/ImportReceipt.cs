using System;
using System.Collections.Generic;

namespace WareManagement.Models;

public partial class ImportReceipt
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public int WarehouseId { get; set; }

    public int? SupplierId { get; set; }

    public string? Type { get; set; }

    public string? Status { get; set; }

    public decimal? TotalAmount { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? ConfirmedAt { get; set; }

    public int? ConfirmedBy { get; set; }

    public DateTime? CancelledAt { get; set; }

    public int? CancelledBy { get; set; }

    public virtual User? CancelledByNavigation { get; set; }

    public virtual User? ConfirmedByNavigation { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<ImportReceiptDetail> ImportReceiptDetails { get; set; } = new List<ImportReceiptDetail>();

    public virtual Partner? Supplier { get; set; }

    public virtual ICollection<Transfer> Transfers { get; set; } = new List<Transfer>();

    public virtual User? UpdatedByNavigation { get; set; }

    public virtual Warehouse Warehouse { get; set; } = null!;
}
