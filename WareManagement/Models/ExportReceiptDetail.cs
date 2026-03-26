using System;
using System.Collections.Generic;

namespace WareManagement.Models;

public partial class ExportReceiptDetail
{
    public int Id { get; set; }

    public int ReceiptId { get; set; }

    public int ProductId { get; set; }

    public decimal? Quantity { get; set; }

    public decimal? Price { get; set; }

    public int LocationId { get; set; }

    public virtual Location Location { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual ExportReceipt Receipt { get; set; } = null!;
}
