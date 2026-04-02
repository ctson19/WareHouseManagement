using Microsoft.EntityFrameworkCore;
using WareManagement.DTO.InventoryDTO;
using WareManagement.Models;
using WareManagement.DTO.ReportDTO;
using WareManagement.Helpers;
using WareManagement.Repository.Interfaces;
using WareManagement.Service.Exceptions;
using WareManagement.Service.Interfaces;

namespace WareManagement.Service.Implementations;

public class ReportService : IReportService
{
    private readonly WareManagementContext _context;
    private readonly IUserRepository _userRepository;
    private readonly IInventoryService _inventoryService;

    public ReportService(WareManagementContext context, IUserRepository userRepository, IInventoryService inventoryService)
    {
        _context = context;
        _userRepository = userRepository;
        _inventoryService = inventoryService;
    }

    private async Task EnsureReportAsync(int userId)
    {
        if (!await _userRepository.CanReadWarehouseDataAsync(userId))
            throw new ForbiddenException("Bạn không có quyền xem báo cáo.");
    }

    public async Task<DashboardSummaryDto> GetDashboardAsync(int userId, CancellationToken cancellationToken = default)
    {
        await EnsureReportAsync(userId);

        var start = DateTime.UtcNow.Date;
        var end = start.AddDays(1);

        var impToday = await _context.ImportReceipts.CountAsync(
            r => r.ConfirmedAt >= start && r.ConfirmedAt < end && r.Status == ReceiptStatuses.Confirmed,
            cancellationToken);

        var expToday = await _context.ExportReceipts.CountAsync(
            r => r.ConfirmedAt >= start && r.ConfirmedAt < end && r.Status == ReceiptStatuses.Confirmed,
            cancellationToken);

        var draftImp = await _context.ImportReceipts.CountAsync(r => r.Status == ReceiptStatuses.Draft, cancellationToken);
        var draftExp = await _context.ExportReceipts.CountAsync(r => r.Status == ReceiptStatuses.Draft, cancellationToken);

        var inv = await _context.Inventories
            .AsNoTracking()
            .Include(i => i.Product)
            .ToListAsync(cancellationToken);

        var grouped = inv
            .GroupBy(i => new { i.ProductId, i.Product!.Code })
            .Select(g => new InventoryTopDto
            {
                ProductId = g.Key.ProductId,
                ProductCode = g.Key.Code ?? "",
                TotalQuantity = g.Sum(x => x.Quantity ?? 0)
            })
            .ToList();

        var topHigh = grouped.OrderByDescending(x => x.TotalQuantity).Take(5).ToList();
        var topLow = grouped.Where(x => x.TotalQuantity > 0).OrderBy(x => x.TotalQuantity).Take(5).ToList();

        return new DashboardSummaryDto
        {
            ImportReceiptsToday = impToday,
            ExportReceiptsToday = expToday,
            DraftImportCount = draftImp,
            DraftExportCount = draftExp,
            TopStockHighest = topHigh,
            TopStockLowest = topLow
        };
    }

    public async Task<List<NxtRowDto>> GetNxtAsync(int userId, DateTime fromUtc, DateTime toUtc, int? warehouseId, CancellationToken cancellationToken = default)
    {
        await EnsureReportAsync(userId);

        var impQ = _context.ImportReceiptDetails
            .AsNoTracking()
            .Include(d => d.Receipt)
            .Include(d => d.Product)
            .Where(d => d.Receipt != null &&
                        d.Receipt.Status == ReceiptStatuses.Confirmed &&
                        d.Receipt.ConfirmedAt >= fromUtc &&
                        d.Receipt.ConfirmedAt <= toUtc);

        var expQ = _context.ExportReceiptDetails
            .AsNoTracking()
            .Include(d => d.Receipt)
            .Include(d => d.Product)
            .Where(d => d.Receipt != null &&
                        d.Receipt.Status == ReceiptStatuses.Confirmed &&
                        d.Receipt.ConfirmedAt >= fromUtc &&
                        d.Receipt.ConfirmedAt <= toUtc);

        if (warehouseId.HasValue)
        {
            var wid = warehouseId.Value;
            impQ = impQ.Where(d => d.Receipt!.WarehouseId == wid);
            expQ = expQ.Where(d => d.Receipt!.WarehouseId == wid);
        }

        var importSums = await impQ
            .GroupBy(d => new { d.ProductId, d.Product!.Code, d.Product.Name })
            .Select(g => new { g.Key.ProductId, g.Key.Code, g.Key.Name, Qty = g.Sum(x => x.Quantity ?? 0) })
            .ToListAsync(cancellationToken);

        var exportSums = await expQ
            .GroupBy(d => new { d.ProductId, d.Product!.Code, d.Product.Name })
            .Select(g => new { g.Key.ProductId, g.Key.Code, g.Key.Name, Qty = g.Sum(x => x.Quantity ?? 0) })
            .ToListAsync(cancellationToken);

        var ids = importSums.Select(x => x.ProductId).Union(exportSums.Select(x => x.ProductId)).Distinct().ToList();

        var rows = new List<NxtRowDto>();
        foreach (var pid in ids)
        {
            var i = importSums.FirstOrDefault(x => x.ProductId == pid);
            var e = exportSums.FirstOrDefault(x => x.ProductId == pid);
            var iq = i?.Qty ?? 0;
            var eq = e?.Qty ?? 0;
            rows.Add(new NxtRowDto
            {
                ProductId = pid,
                ProductCode = i?.Code ?? e?.Code ?? "",
                ProductName = i?.Name ?? e?.Name,
                ImportQty = iq,
                ExportQty = eq,
                NetChange = iq - eq
            });
        }

        return rows.OrderBy(r => r.ProductCode).ToList();
    }

    public async Task<List<StockTransactionRowDto>> GetProductTransactionHistoryAsync(int userId, int productId, int? warehouseId, CancellationToken cancellationToken = default)
    {
        await EnsureReportAsync(userId);

        var q = _context.StockTransactions
            .AsNoTracking()
            .Include(t => t.Warehouse)
            .Include(t => t.Product)
            .Where(t => t.ProductId == productId);

        if (warehouseId.HasValue)
            q = q.Where(t => t.WarehouseId == warehouseId.Value);

        var list = await q
            .OrderByDescending(t => t.CreatedAt)
            .Take(500)
            .ToListAsync(cancellationToken);

        return list.Select(t => new StockTransactionRowDto
        {
            Id = t.Id,
            CreatedAt = t.CreatedAt,
            TransactionType = t.TransactionType,
            Quantity = t.Quantity,
            ReferenceType = t.ReferenceType,
            ReferenceId = t.ReferenceId,
            WarehouseId = t.WarehouseId,
            WarehouseCode = t.Warehouse?.Code,
            ProductId = t.ProductId,
            ProductCode = t.Product?.Code
        }).ToList();
    }

    public async Task<List<InventorySummaryDto>> GetLowStockAsync(int userId, int? warehouseId, CancellationToken cancellationToken = default)
    {
        await EnsureReportAsync(userId);

        var all = await _inventoryService.GetSummaryAsync(userId, warehouseId, null, null, cancellationToken);
        return all.Where(x => x.StockAlert == "BelowMin").ToList();
    }

    public async Task<List<PartnerReceiptRowDto>> GetReceiptsByPartnerAsync(int userId, int partnerId, CancellationToken cancellationToken = default)
    {
        await EnsureReportAsync(userId);

        var partner = await _context.Partners.AsNoTracking().FirstOrDefaultAsync(p => p.Id == partnerId, cancellationToken);
        if (partner is null) throw new NotFoundException("Không tìm thấy đối tác.");

        var rows = new List<PartnerReceiptRowDto>();

        if (partner.Type == PartnerTypes.Supplier)
        {
            var imps = await _context.ImportReceipts
                .AsNoTracking()
                .Where(r => r.SupplierId == partnerId)
                .OrderByDescending(r => r.CreatedAt)
                .Take(200)
                .ToListAsync(cancellationToken);

            rows.AddRange(imps.Select(r => new PartnerReceiptRowDto
            {
                ReceiptKind = "Import",
                ReceiptId = r.Id,
                Code = r.Code,
                Date = r.ConfirmedAt ?? r.CreatedAt,
                TotalAmount = r.TotalAmount,
                Status = r.Status
            }));
        }
        else if (partner.Type == PartnerTypes.Customer)
        {
            var exps = await _context.ExportReceipts
                .AsNoTracking()
                .Where(r => r.CustomerId == partnerId)
                .OrderByDescending(r => r.CreatedAt)
                .Take(200)
                .ToListAsync(cancellationToken);

            rows.AddRange(exps.Select(r => new PartnerReceiptRowDto
            {
                ReceiptKind = "Export",
                ReceiptId = r.Id,
                Code = r.Code,
                Date = r.ConfirmedAt ?? r.CreatedAt,
                TotalAmount = null,
                Status = r.Status
            }));
        }

        return rows;
    }
}
