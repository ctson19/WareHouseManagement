using System;
using System.Collections.Generic;

namespace WareManagement.Models;

public partial class Partner
{
    public int Id { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public string Type { get; set; } = null!;

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? TaxCode { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<ExportReceipt> ExportReceipts { get; set; } = new List<ExportReceipt>();

    public virtual ICollection<ImportReceipt> ImportReceipts { get; set; } = new List<ImportReceipt>();
}
