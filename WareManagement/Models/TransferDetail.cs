using System;
using System.Collections.Generic;

namespace WareManagement.Models;

public partial class TransferDetail
{
    public int Id { get; set; }

    public int? TransferId { get; set; }

    public int? ProductId { get; set; }

    public decimal? Quantity { get; set; }

    public virtual Product? Product { get; set; }

    public virtual Transfer? Transfer { get; set; }
}
