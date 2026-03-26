using System;
using System.Collections.Generic;

namespace WareManagement.Models;

public partial class ExportReceipt
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public int WarehouseId { get; set; }

    public int? CustomerId { get; set; }

    public string? Type { get; set; }

    public string? Status { get; set; }

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

    public virtual Partner? Customer { get; set; }

    public virtual ICollection<ExportReceiptDetail> ExportReceiptDetails { get; set; } = new List<ExportReceiptDetail>();

    public virtual ICollection<Transfer> Transfers { get; set; } = new List<Transfer>();

    public virtual User? UpdatedByNavigation { get; set; }

    public virtual Warehouse Warehouse { get; set; } = null!;
}
