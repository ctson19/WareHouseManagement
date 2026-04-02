using Microsoft.EntityFrameworkCore;
using WareManagement.DTO.TransferDTO;
using WareManagement.DTO.NotificationDTO;
using WareManagement.Helpers;
using WareManagement.Models;
using WareManagement.Repository.Interfaces;
using WareManagement.Service.Exceptions;
using WareManagement.Service.Interfaces;

namespace WareManagement.Service.Implementations;

public class TransferService : ITransferService
{
    private readonly WareManagementContext _context;
    private readonly IUserRepository _userRepository;
    private readonly IStockService _stockService;
    private readonly INotificationService _notificationService;

    public TransferService(
        WareManagementContext context,
        IUserRepository userRepository,
        IStockService stockService,
        INotificationService notificationService)
    {
        _context = context;
        _userRepository = userRepository;
        _stockService = stockService;
        _notificationService = notificationService;
    }

    private async Task EnsureManageAsync(int userId)
    {
        if (!await _userRepository.CanManageCatalogAsync(userId))
            throw new ForbiddenException("Bạn không có quyền quản lý điều chuyển.");
    }

    private async Task EnsureReadAsync(int userId)
    {
        if (!await _userRepository.CanReadWarehouseDataAsync(userId))
            throw new ForbiddenException("Bạn không có quyền xem.");
    }

    private async Task<string> NextExportCodeAsync(CancellationToken cancellationToken)
    {
        var prefix = "PX" + DateTime.UtcNow.ToString("yyyyMMdd") + "TR";
        var count = await _context.ExportReceipts.CountAsync(r => r.Code.StartsWith(prefix), cancellationToken);
        return $"{prefix}{(count + 1):D4}";
    }

    private async Task<string> NextImportCodeAsync(CancellationToken cancellationToken)
    {
        var prefix = "PN" + DateTime.UtcNow.ToString("yyyyMMdd") + "TR";
        var count = await _context.ImportReceipts.CountAsync(r => r.Code.StartsWith(prefix), cancellationToken);
        return $"{prefix}{(count + 1):D4}";
    }

    private async Task<TransferResponseDto> MapAsync(Transfer t, CancellationToken cancellationToken)
    {
        await _context.Entry(t).Collection(x => x.TransferDetails).Query()
            .Include(d => d.Product)
            .LoadAsync(cancellationToken);

        var from = await _context.Warehouses.AsNoTracking().FirstOrDefaultAsync(w => w.Id == t.FromWarehouseId, cancellationToken);
        var to = await _context.Warehouses.AsNoTracking().FirstOrDefaultAsync(w => w.Id == t.ToWarehouseId, cancellationToken);

        var lines = t.TransferDetails
            .OrderBy(d => d.Id)
            .Select(d => new TransferDetailResponseDto
            {
                Id = d.Id,
                ProductId = d.ProductId,
                ProductCode = d.Product?.Code,
                Quantity = d.Quantity
            }).ToList();

        return new TransferResponseDto
        {
            Id = t.Id,
            FromWarehouseId = t.FromWarehouseId,
            FromWarehouseCode = from?.Code,
            ToWarehouseId = t.ToWarehouseId,
            ToWarehouseCode = to?.Code,
            Status = t.Status,
            ExportReceiptId = t.ExportReceiptId,
            ImportReceiptId = t.ImportReceiptId,
            CreatedAt = t.CreatedAt,
            Lines = lines
        };
    }

    public async Task<(List<TransferResponseDto> Items, int Total)> GetPagedAsync(int userId, string? status, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        await EnsureReadAsync(userId);
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 200) pageSize = 200;

        var q = _context.Transfers.AsQueryable();
        if (!string.IsNullOrWhiteSpace(status))
            q = q.Where(t => t.Status == status.Trim());

        var total = await q.CountAsync(cancellationToken);
        var ids = await q.OrderByDescending(t => t.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).Select(t => t.Id).ToListAsync(cancellationToken);

        var list = new List<TransferResponseDto>();
        foreach (var id in ids)
        {
            var entity = await _context.Transfers.FirstAsync(t => t.Id == id, cancellationToken);
            list.Add(await MapAsync(entity, cancellationToken));
        }

        return (list, total);
    }

    public async Task<TransferResponseDto> GetByIdAsync(int userId, int id, CancellationToken cancellationToken = default)
    {
        await EnsureReadAsync(userId);
        var t = await _context.Transfers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (t is null) throw new NotFoundException("Không tìm thấy phiếu điều chuyển.");

        return await MapAsync(t, cancellationToken);
    }

    public async Task<TransferResponseDto> CreateAsync(int userId, CreateTransferRequestDto request, CancellationToken cancellationToken = default)
    {
        await EnsureManageAsync(userId);

        if (request is null) throw new ValidationException("Dữ liệu không hợp lệ.");
        if (request.FromWarehouseId == request.ToWarehouseId)
            throw new ValidationException("Kho nguồn và kho đích phải khác nhau.");
        if (request.Lines is null || request.Lines.Count == 0)
            throw new ValidationException("Phải có ít nhất một dòng hàng.");

        var fw = await _context.Warehouses.AnyAsync(w => w.Id == request.FromWarehouseId, cancellationToken);
        var tw = await _context.Warehouses.AnyAsync(w => w.Id == request.ToWarehouseId, cancellationToken);
        if (!fw || !tw) throw new ValidationException("Kho không tồn tại.");

        foreach (var line in request.Lines)
        {
            if (line.Quantity <= 0) throw new ValidationException("Số lượng phải lớn hơn 0.");
            var p = await _context.Products.AnyAsync(x => x.Id == line.ProductId, cancellationToken);
            if (!p) throw new ValidationException($"Hàng Id={line.ProductId} không tồn tại.");
        }

        var transfer = new Transfer
        {
            FromWarehouseId = request.FromWarehouseId,
            ToWarehouseId = request.ToWarehouseId,
            Status = TransferStatuses.Pending,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

        _context.Transfers.Add(transfer);
        await _context.SaveChangesAsync(cancellationToken);

        foreach (var line in request.Lines)
        {
            _context.TransferDetails.Add(new TransferDetail
            {
                TransferId = transfer.Id,
                ProductId = line.ProductId,
                Quantity = line.Quantity
            });
        }

        await _context.SaveChangesAsync(cancellationToken);

        var tracked = await _context.Transfers.FirstAsync(t => t.Id == transfer.Id, cancellationToken);
        return await MapAsync(tracked, cancellationToken);
    }

    public async Task<TransferResponseDto> ConfirmAsync(int userId, int id, TransferConfirmRequestDto request, CancellationToken cancellationToken = default)
    {
        await EnsureManageAsync(userId);

        if (request?.Lines is null || request.Lines.Count == 0)
            throw new ValidationException("Cần gửi vị trí xuất/nhập cho từng dòng.");

        await using var tx = await _context.Database.BeginTransactionAsync(cancellationToken);

        var transfer = await _context.Transfers
            .Include(t => t.TransferDetails)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (transfer is null) throw new NotFoundException("Không tìm thấy phiếu điều chuyển.");
        if (transfer.Status != TransferStatuses.Pending && transfer.Status != TransferStatuses.InTransit)
            throw new ValidationException("Chỉ xác nhận khi phiếu đang chờ hoặc đang vận chuyển.");

        var details = transfer.TransferDetails.Where(d => d.ProductId.HasValue).ToList();
        if (details.Count != request.Lines.Count)
            throw new ValidationException("Số dòng xác nhận không khớp chi tiết phiếu.");

        foreach (var line in request.Lines)
        {
            var detail = details.FirstOrDefault(d => d.Id == line.DetailId);
            if (detail is null) throw new ValidationException($"Detail Id={line.DetailId} không hợp lệ.");

            var fromLoc = await _context.Locations.FirstOrDefaultAsync(l => l.Id == line.FromLocationId, cancellationToken);
            var toLoc = await _context.Locations.FirstOrDefaultAsync(l => l.Id == line.ToLocationId, cancellationToken);

            if (fromLoc is null || fromLoc.WarehouseId != transfer.FromWarehouseId)
                throw new ValidationException($"Vị trí xuất Id={line.FromLocationId} không thuộc kho nguồn.");
            if (toLoc is null || toLoc.WarehouseId != transfer.ToWarehouseId)
                throw new ValidationException($"Vị trí nhập Id={line.ToLocationId} không thuộc kho đích.");
        }

        var exportCode = await NextExportCodeAsync(cancellationToken);
        var importCode = await NextImportCodeAsync(cancellationToken);

        var export = new ExportReceipt
        {
            Code = exportCode,
            WarehouseId = transfer.FromWarehouseId,
            CustomerId = null,
            Type = ExportReceiptTypes.TransferOut,
            Status = ReceiptStatuses.Confirmed,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId,
            ConfirmedAt = DateTime.UtcNow,
            ConfirmedBy = userId
        };

        _context.ExportReceipts.Add(export);
        await _context.SaveChangesAsync(cancellationToken);

        var import = new ImportReceipt
        {
            Code = importCode,
            WarehouseId = transfer.ToWarehouseId,
            SupplierId = null,
            Type = ImportReceiptTypes.TransferIn,
            Status = ReceiptStatuses.Confirmed,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId,
            ConfirmedAt = DateTime.UtcNow,
            ConfirmedBy = userId,
            TotalAmount = 0
        };

        _context.ImportReceipts.Add(import);
        await _context.SaveChangesAsync(cancellationToken);

        decimal importTotal = 0;

        foreach (var line in request.Lines)
        {
            var detail = details.First(d => d.Id == line.DetailId);
            var qty = detail.Quantity ?? 0;
            var productId = detail.ProductId!.Value;

            var product = await _context.Products.FirstAsync(p => p.Id == productId, cancellationToken);
            var price = product.ImportPrice ?? 0;

            _context.ExportReceiptDetails.Add(new ExportReceiptDetail
            {
                ReceiptId = export.Id,
                ProductId = productId,
                Quantity = qty,
                Price = price,
                LocationId = line.FromLocationId
            });

            _context.ImportReceiptDetails.Add(new ImportReceiptDetail
            {
                ReceiptId = import.Id,
                ProductId = productId,
                Quantity = qty,
                Price = price,
                LocationId = line.ToLocationId
            });

            importTotal += qty * price;
        }

        import.TotalAmount = importTotal;
        await _context.SaveChangesAsync(cancellationToken);

        foreach (var line in request.Lines)
        {
            var detail = details.First(d => d.Id == line.DetailId);
            var qty = detail.Quantity ?? 0;
            var productId = detail.ProductId!.Value;

            await _stockService.ApplyInventoryDeltaAsync(
                transfer.FromWarehouseId,
                productId,
                line.FromLocationId,
                -qty,
                StockReferenceTypes.ExportReceipt,
                export.Id,
                userId,
                cancellationToken);

            await _stockService.ApplyInventoryDeltaAsync(
                transfer.ToWarehouseId,
                productId,
                line.ToLocationId,
                qty,
                StockReferenceTypes.ImportReceipt,
                import.Id,
                userId,
                cancellationToken);
        }

        transfer.ExportReceiptId = export.Id;
        transfer.ImportReceiptId = import.Id;
        transfer.Status = TransferStatuses.Received;

        await _context.SaveChangesAsync(cancellationToken);

        await _notificationService.CreateForUserAsync(userId, new CreateNotificationRequestDto
        {
            Title = "Phiếu điều chuyển đã hoàn tất",
            Content = $"Phiếu điều chuyển ID={transfer.Id} đã được nhận thành công.",
            Type = "TransferReceived",
            ReferenceId = transfer.Id,
            ReferenceType = "Transfer"
        });

        await tx.CommitAsync(cancellationToken);

        return await GetByIdAsync(userId, id, cancellationToken);
    }

    public async Task SetStatusAsync(int userId, int id, string status, CancellationToken cancellationToken = default)
    {
        await EnsureManageAsync(userId);

        if (string.IsNullOrWhiteSpace(status))
            throw new ValidationException("Trạng thái không hợp lệ.");

        var s = status.Trim();
        if (s != TransferStatuses.InTransit)
            throw new ValidationException("Chỉ hỗ trợ chuyển sang Đang vận chuyển (InTransit).");

        var transfer = await _context.Transfers.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (transfer is null) throw new NotFoundException("Không tìm thấy phiếu điều chuyển.");

        if (transfer.Status == TransferStatuses.Received)
            throw new ValidationException("Phiếu đã hoàn tất, không đổi trạng thái.");

        if (transfer.Status != TransferStatuses.Pending)
            throw new ValidationException("Chỉ chuyển sang Đang vận chuyển khi phiếu đang ở trạng thái chờ.");

        transfer.Status = TransferStatuses.InTransit;
        await _context.SaveChangesAsync(cancellationToken);

        await _notificationService.CreateForUserAsync(userId, new CreateNotificationRequestDto
        {
            Title = "Phiếu điều chuyển đang vận chuyển",
            Content = $"Phiếu điều chuyển ID={transfer.Id} đã chuyển sang trạng thái InTransit.",
            Type = "TransferInTransit",
            ReferenceId = transfer.Id,
            ReferenceType = "Transfer"
        });
    }
}
