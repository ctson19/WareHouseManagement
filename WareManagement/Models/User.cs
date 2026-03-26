using System;
using System.Collections.Generic;

namespace WareManagement.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    public virtual ICollection<ExportReceipt> ExportReceiptCancelledByNavigations { get; set; } = new List<ExportReceipt>();

    public virtual ICollection<ExportReceipt> ExportReceiptConfirmedByNavigations { get; set; } = new List<ExportReceipt>();

    public virtual ICollection<ExportReceipt> ExportReceiptCreatedByNavigations { get; set; } = new List<ExportReceipt>();

    public virtual ICollection<ExportReceipt> ExportReceiptUpdatedByNavigations { get; set; } = new List<ExportReceipt>();

    public virtual ICollection<ImportReceipt> ImportReceiptCancelledByNavigations { get; set; } = new List<ImportReceipt>();

    public virtual ICollection<ImportReceipt> ImportReceiptConfirmedByNavigations { get; set; } = new List<ImportReceipt>();

    public virtual ICollection<ImportReceipt> ImportReceiptCreatedByNavigations { get; set; } = new List<ImportReceipt>();

    public virtual ICollection<ImportReceipt> ImportReceiptUpdatedByNavigations { get; set; } = new List<ImportReceipt>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Partner> Partners { get; set; } = new List<Partner>();

    public virtual ICollection<Product> ProductCreatedByNavigations { get; set; } = new List<Product>();

    public virtual ICollection<Product> ProductUpdatedByNavigations { get; set; } = new List<Product>();

    public virtual ICollection<StockCount> StockCounts { get; set; } = new List<StockCount>();

    public virtual ICollection<StockTransaction> StockTransactions { get; set; } = new List<StockTransaction>();

    public virtual ICollection<Transfer> Transfers { get; set; } = new List<Transfer>();

    public virtual ICollection<UserLoginLog> UserLoginLogs { get; set; } = new List<UserLoginLog>();

    public virtual ICollection<Warehouse> WarehouseCreatedByNavigations { get; set; } = new List<Warehouse>();

    public virtual ICollection<Warehouse> WarehouseUpdatedByNavigations { get; set; } = new List<Warehouse>();

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
