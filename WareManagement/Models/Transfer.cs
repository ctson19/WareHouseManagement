using System;
using System.Collections.Generic;

namespace WareManagement.Models;

public partial class Transfer
{
    public int Id { get; set; }

    public int FromWarehouseId { get; set; }

    public int ToWarehouseId { get; set; }

    public string? Status { get; set; }

    public int? ExportReceiptId { get; set; }

    public int? ImportReceiptId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ExportReceipt? ExportReceipt { get; set; }

    public virtual Warehouse FromWarehouse { get; set; } = null!;

    public virtual ImportReceipt? ImportReceipt { get; set; }

    public virtual Warehouse ToWarehouse { get; set; } = null!;

    public virtual ICollection<TransferDetail> TransferDetails { get; set; } = new List<TransferDetail>();
}
