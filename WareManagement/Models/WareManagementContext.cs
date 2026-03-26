using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WareManagement.Models;

public partial class WareManagementContext : DbContext
{
    public WareManagementContext()
    {
    }

    public WareManagementContext(DbContextOptions<WareManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<ExportReceipt> ExportReceipts { get; set; }

    public virtual DbSet<ExportReceiptDetail> ExportReceiptDetails { get; set; }

    public virtual DbSet<ImportReceipt> ImportReceipts { get; set; }

    public virtual DbSet<ImportReceiptDetail> ImportReceiptDetails { get; set; }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Partner> Partners { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<StockCount> StockCounts { get; set; }

    public virtual DbSet<StockCountDetail> StockCountDetails { get; set; }

    public virtual DbSet<StockTransaction> StockTransactions { get; set; }

    public virtual DbSet<Transfer> Transfers { get; set; }

    public virtual DbSet<TransferDetail> TransferDetails { get; set; }

    public virtual DbSet<Unit> Units { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserLoginLog> UserLoginLogs { get; set; }

    public virtual DbSet<Warehouse> Warehouses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AuditLog__3214EC0739350A9E");

            entity.Property(e => e.Action).HasMaxLength(255);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TableName).HasMaxLength(100);

            entity.HasOne(d => d.User).WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__AuditLogs__UserI__3587F3E0");
        });

        modelBuilder.Entity<ExportReceipt>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ExportRe__3214EC074D5E1625");

            entity.HasIndex(e => e.Code, "UQ__ExportRe__A25C5AA74F591DBA").IsUnique();

            entity.Property(e => e.CancelledAt).HasColumnType("datetime");
            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.ConfirmedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.CancelledByNavigation).WithMany(p => p.ExportReceiptCancelledByNavigations)
                .HasForeignKey(d => d.CancelledBy)
                .HasConstraintName("FK__ExportRec__Cance__17F790F9");

            entity.HasOne(d => d.ConfirmedByNavigation).WithMany(p => p.ExportReceiptConfirmedByNavigations)
                .HasForeignKey(d => d.ConfirmedBy)
                .HasConstraintName("FK__ExportRec__Confi__17036CC0");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ExportReceiptCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__ExportRec__Creat__151B244E");

            entity.HasOne(d => d.Customer).WithMany(p => p.ExportReceipts)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__ExportRec__Custo__14270015");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ExportReceiptUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__ExportRec__Updat__160F4887");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.ExportReceipts)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ExportRec__Wareh__1332DBDC");
        });

        modelBuilder.Entity<ExportReceiptDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ExportRe__3214EC07EACFC3C4");

            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Location).WithMany(p => p.ExportReceiptDetails)
                .HasForeignKey(d => d.LocationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ExportRec__Locat__1CBC4616");

            entity.HasOne(d => d.Product).WithMany(p => p.ExportReceiptDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ExportRec__Produ__1BC821DD");

            entity.HasOne(d => d.Receipt).WithMany(p => p.ExportReceiptDetails)
                .HasForeignKey(d => d.ReceiptId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ExportRec__Recei__1AD3FDA4");
        });

        modelBuilder.Entity<ImportReceipt>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ImportRe__3214EC07880FAF54");

            entity.HasIndex(e => e.Code, "UQ__ImportRe__A25C5AA780049CEC").IsUnique();

            entity.Property(e => e.CancelledAt).HasColumnType("datetime");
            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.ConfirmedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.CancelledByNavigation).WithMany(p => p.ImportReceiptCancelledByNavigations)
                .HasForeignKey(d => d.CancelledBy)
                .HasConstraintName("FK__ImportRec__Cance__09A971A2");

            entity.HasOne(d => d.ConfirmedByNavigation).WithMany(p => p.ImportReceiptConfirmedByNavigations)
                .HasForeignKey(d => d.ConfirmedBy)
                .HasConstraintName("FK__ImportRec__Confi__08B54D69");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ImportReceiptCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__ImportRec__Creat__06CD04F7");

            entity.HasOne(d => d.Supplier).WithMany(p => p.ImportReceipts)
                .HasForeignKey(d => d.SupplierId)
                .HasConstraintName("FK__ImportRec__Suppl__05D8E0BE");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ImportReceiptUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__ImportRec__Updat__07C12930");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.ImportReceipts)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ImportRec__Wareh__04E4BC85");
        });

        modelBuilder.Entity<ImportReceiptDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ImportRe__3214EC07059EFB8A");

            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Location).WithMany(p => p.ImportReceiptDetails)
                .HasForeignKey(d => d.LocationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ImportRec__Locat__0E6E26BF");

            entity.HasOne(d => d.Product).WithMany(p => p.ImportReceiptDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ImportRec__Produ__0D7A0286");

            entity.HasOne(d => d.Receipt).WithMany(p => p.ImportReceiptDetails)
                .HasForeignKey(d => d.ReceiptId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ImportRec__Recei__0C85DE4D");
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Inventor__3214EC078CEB2FAA");

            entity.HasIndex(e => e.ProductId, "IX_Inventory_Product");

            entity.HasIndex(e => e.WarehouseId, "IX_Inventory_Warehouse");

            entity.HasIndex(e => new { e.WarehouseId, e.ProductId, e.LocationId }, "UQ__Inventor__7BAF9D3044012740").IsUnique();

            entity.Property(e => e.Quantity)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Location).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.LocationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventori__Locat__797309D9");

            entity.HasOne(d => d.Product).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventori__Produ__787EE5A0");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventori__Wareh__778AC167");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Location__3214EC078B6CF1B4");

            entity.HasIndex(e => e.ParentId, "IX_Location_Parent");

            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Type).HasMaxLength(50);

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK__Locations__Paren__619B8048");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.Locations)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Locations__Wareh__60A75C0F");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Notifica__3214EC07D1AE332A");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsRead).HasDefaultValue(false);
            entity.Property(e => e.ReferenceType).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.Type).HasMaxLength(50);

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificat__UserI__3E1D39E1");
        });

        modelBuilder.Entity<Partner>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Partners__3214EC07F2048B42");

            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.TaxCode).HasMaxLength(50);
            entity.Property(e => e.Type).HasMaxLength(50);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Partners)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Partners__Create__72C60C4A");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Permissi__3214EC072797B4F9");

            entity.Property(e => e.Code).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Products__3214EC07CB7C24C4");

            entity.HasIndex(e => e.Code, "UQ__Products__A25C5AA7C98C7CED").IsUnique();

            entity.Property(e => e.Barcode).HasMaxLength(100);
            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ImportPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.MaxStock)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MinStock)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.SalePrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ProductCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Products__Create__6D0D32F4");

            entity.HasOne(d => d.Unit).WithMany(p => p.Products)
                .HasForeignKey(d => d.UnitId)
                .HasConstraintName("FK__Products__UnitId__6C190EBB");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ProductUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__Products__Update__6E01572D");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC07FECE6580");

            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasMany(d => d.Permissions).WithMany(p => p.Roles)
                .UsingEntity<Dictionary<string, object>>(
                    "RolePermission",
                    r => r.HasOne<Permission>().WithMany()
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__RolePermi__Permi__571DF1D5"),
                    l => l.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__RolePermi__RoleI__5629CD9C"),
                    j =>
                    {
                        j.HasKey("RoleId", "PermissionId").HasName("PK__RolePerm__6400A1A8F2636F91");
                        j.ToTable("RolePermissions");
                    });
        });

        modelBuilder.Entity<StockCount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__StockCou__3214EC07C5A9542B");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.StockCounts)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__StockCoun__Creat__2CF2ADDF");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.StockCounts)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StockCoun__Wareh__2BFE89A6");
        });

        modelBuilder.Entity<StockCountDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__StockCou__3214EC073902E8F7");

            entity.Property(e => e.ActualQty).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DifferenceQty)
                .HasComputedColumnSql("([ActualQty]-[SystemQty])", false)
                .HasColumnType("decimal(19, 2)");
            entity.Property(e => e.SystemQty).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Location).WithMany(p => p.StockCountDetails)
                .HasForeignKey(d => d.LocationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StockCoun__Locat__31B762FC");

            entity.HasOne(d => d.Product).WithMany(p => p.StockCountDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StockCoun__Produ__30C33EC3");

            entity.HasOne(d => d.StockCount).WithMany(p => p.StockCountDetails)
                .HasForeignKey(d => d.StockCountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StockCoun__Stock__2FCF1A8A");
        });

        modelBuilder.Entity<StockTransaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__StockTra__3214EC071CAF153C");

            entity.HasIndex(e => e.ProductId, "IX_Stock_Product");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ReferenceType).HasMaxLength(50);
            entity.Property(e => e.TransactionType).HasMaxLength(50);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.StockTransactions)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__StockTran__Creat__00200768");

            entity.HasOne(d => d.Location).WithMany(p => p.StockTransactions)
                .HasForeignKey(d => d.LocationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StockTran__Locat__7F2BE32F");

            entity.HasOne(d => d.Product).WithMany(p => p.StockTransactions)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StockTran__Produ__7D439ABD");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.StockTransactions)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StockTran__Wareh__7E37BEF6");
        });

        modelBuilder.Entity<Transfer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Transfer__3214EC07A43EEB88");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Transfers)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Transfers__Creat__245D67DE");

            entity.HasOne(d => d.ExportReceipt).WithMany(p => p.Transfers)
                .HasForeignKey(d => d.ExportReceiptId)
                .HasConstraintName("FK__Transfers__Expor__22751F6C");

            entity.HasOne(d => d.FromWarehouse).WithMany(p => p.TransferFromWarehouses)
                .HasForeignKey(d => d.FromWarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Transfers__FromW__208CD6FA");

            entity.HasOne(d => d.ImportReceipt).WithMany(p => p.Transfers)
                .HasForeignKey(d => d.ImportReceiptId)
                .HasConstraintName("FK__Transfers__Impor__236943A5");

            entity.HasOne(d => d.ToWarehouse).WithMany(p => p.TransferToWarehouses)
                .HasForeignKey(d => d.ToWarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Transfers__ToWar__2180FB33");
        });

        modelBuilder.Entity<TransferDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Transfer__3214EC07C2E3A041");

            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Product).WithMany(p => p.TransferDetails)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__TransferD__Produ__282DF8C2");

            entity.HasOne(d => d.Transfer).WithMany(p => p.TransferDetails)
                .HasForeignKey(d => d.TransferId)
                .HasConstraintName("FK__TransferD__Trans__2739D489");
        });

        modelBuilder.Entity<Unit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Units__3214EC0747879064");

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC070CB6E5B6");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E431B038D8").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.Username).HasMaxLength(100);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    r => r.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__UserRoles__RoleI__534D60F1"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__UserRoles__UserI__52593CB8"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId").HasName("PK__UserRole__AF2760ADB6A4BE5C");
                        j.ToTable("UserRoles");
                    });
        });

        modelBuilder.Entity<UserLoginLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserLogi__3214EC0729B7C56D");

            entity.Property(e => e.Ipaddress)
                .HasMaxLength(50)
                .HasColumnName("IPAddress");
            entity.Property(e => e.LoginTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.UserLoginLogs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__UserLogin__UserI__395884C4");
        });

        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Warehous__3214EC0791DB4555");

            entity.HasIndex(e => e.Code, "UQ__Warehous__A25C5AA71407C0E1").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Status).HasDefaultValue(true);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.WarehouseCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Warehouse__Creat__5CD6CB2B");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.WarehouseUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__Warehouse__Updat__5DCAEF64");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
