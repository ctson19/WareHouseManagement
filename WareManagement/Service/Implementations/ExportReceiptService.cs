using Microsoft.EntityFrameworkCore;
using WareManagement.DTO.ExportReceiptDTO;
using WareManagement.DTO.NotificationDTO;
using WareManagement.Helpers;
using WareManagement.Models;
using WareManagement.Repository.Interfaces;
using WareManagement.Service.Exceptions;
using WareManagement.Service.Interfaces;

namespace WareManagement.Service.Implementations;

public class ExportReceiptService : IExportReceiptService
{
    private readonly WareManagementContext _context;
    private readonly IUserRepository _userRepository;
    private readonly IStockService _stockService;
    private readonly INotificationService _notificationService;

    public ExportReceiptService(
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
            throw new ForbiddenException("Bạn không có quyền quản lý phiếu xuất.");
    }

    private async Task EnsureReadAsync(int userId)
    {
        if (!await _userRepository.CanReadWarehouseDataAsync(userId))
            throw new ForbiddenException("Bạn không có quyền xem.");
    }

    private async Task<string> NextCodeAsync(CancellationToken cancellationToken)
    {
        var prefix = "PX" + DateTime.UtcNow.ToString("yyyyMMdd");
        var count = await _context.ExportReceipts.CountAsync(r => r.Code.StartsWith(prefix), cancellationToken);
        return $"{prefix}{(count + 1):D4}";
    }

    private static void ValidateLines(List<ExportReceiptLineDto> lines)
    {
        if (lines is null || lines.Count == 0)
            throw new ValidationException("Phiếu xuất phải có ít nhất một dòng hàng.");

        foreach (var line in lines)
        {
            if (line.Quantity <= 0) throw new ValidationException("Số lượng phải lớn hơn 0.");
            if (line.Price < 0) throw new ValidationException("Đơn giá không hợp lệ.");
        }
    }

    private static readonly string[] ValidTypes =
    {
        ExportReceiptTypes.Sale,
        ExportReceiptTypes.TransferOut,
        ExportReceiptTypes.StockAdjustDecrease
    };

    private async Task ValidateHeaderAndLinesAsync(ExportReceipt receipt, List<ExportReceiptLineDto> lines, CancellationToken cancellationToken)
    {
        ValidateLines(lines);

        if (!ValidTypes.Contains(receipt.Type))
            throw new ValidationException("Loại phiếu xuất không hợp lệ.");

        if (receipt.Type == ExportReceiptTypes.Sale && receipt.CustomerId is null)
            throw new ValidationException("Xuất bán phải chọn khách hàng.");

        var wh = await _context.Warehouses.FirstOrDefaultAsync(w => w.Id == receipt.WarehouseId, cancellationToken);
        if (wh is null) throw new ValidationException("Kho không tồn tại.");

        foreach (var line in lines)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == line.ProductId, cancellationToken);
            if (product is null) throw new ValidationException($"Hàng Id={line.ProductId} không tồn tại.");

            var loc = await _context.Locations.FirstOrDefaultAsync(l => l.Id == line.LocationId, cancellationToken);
            if (loc is null || loc.WarehouseId != receipt.WarehouseId)
                throw new ValidationException($"Vị trí Id={line.LocationId} không thuộc kho xuất.");
        }

        if (receipt.CustomerId.HasValue)
        {
            var c = await _context.Partners.FirstOrDefaultAsync(p => p.Id == receipt.CustomerId && p.Type == PartnerTypes.Customer, cancellationToken);
            if (c is null) throw new ValidationException("Khách hàng không hợp lệ.");
        }
    }

    private async Task<ExportReceiptResponseDto> MapAsync(ExportReceipt r, CancellationToken cancellationToken)
    {
        await _context.Entry(r).Collection(x => x.ExportReceiptDetails).Query()
            .Include(d => d.Product)
            .Include(d => d.Location)
            .LoadAsync(cancellationToken);

        var wh = await _context.Warehouses.AsNoTracking().FirstOrDefaultAsync(w => w.Id == r.WarehouseId, cancellationToken);
        string? custName = null;
        if (r.CustomerId.HasValue)
        {
            var c = await _context.Partners.AsNoTracking().FirstOrDefaultAsync(p => p.Id == r.CustomerId, cancellationToken);
            custName = c?.Name;
        }

        var lines = r.ExportReceiptDetails.OrderBy(d => d.Id).Select(d => new ExportReceiptDetailResponseDto
        {
            Id = d.Id,
            ProductId = d.ProductId,
            ProductCode = d.Product?.Code,
            ProductName = d.Product?.Name,
            Quantity = d.Quantity ?? 0,
            Price = d.Price ?? 0,
            LineAmount = (d.Quantity ?? 0) * (d.Price ?? 0),
            LocationId = d.LocationId,
            LocationName = d.Location?.Name
        }).ToList();

        var total = lines.Sum(x => x.LineAmount);

        return new ExportReceiptResponseDto
        {
            Id = r.Id,
            Code = r.Code,
            WarehouseId = r.WarehouseId,
            WarehouseCode = wh?.Code,
            CustomerId = r.CustomerId,
            CustomerName = custName,
            Type = r.Type,
            Status = r.Status,
            TotalAmount = total,
            CreatedAt = r.CreatedAt,
            Lines = lines
        };
    }

    public async Task<(List<ExportReceiptResponseDto> Items, int Total)> GetPagedAsync(int userId, int? warehouseId, string? status, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        await EnsureReadAsync(userId);
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 200) pageSize = 200;

        var q = _context.ExportReceipts.AsNoTracking().AsQueryable();
        if (warehouseId.HasValue) q = q.Where(r => r.WarehouseId == warehouseId.Value);
        if (!string.IsNullOrWhiteSpace(status)) q = q.Where(r => r.Status == status.Trim());

        var total = await q.CountAsync(cancellationToken);
        var ids = await q.OrderByDescending(r => r.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).Select(r => r.Id).ToListAsync(cancellationToken);

        var list = new List<ExportReceiptResponseDto>();
        foreach (var id in ids)
        {
            var entity = await _context.ExportReceipts.FirstAsync(r => r.Id == id, cancellationToken);
            list.Add(await MapAsync(entity, cancellationToken));
        }

        return (list, total);
    }

    public async Task<ExportReceiptResponseDto> GetByIdAsync(int userId, int id, CancellationToken cancellationToken = default)
    {
        await EnsureReadAsync(userId);
        var r = await _context.ExportReceipts.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (r is null) throw new NotFoundException("Không tìm thấy phiếu xuất.");

        return await MapAsync(r, cancellationToken);
    }

    public async Task<ExportReceiptResponseDto> CreateAsync(int userId, CreateExportReceiptRequestDto request, CancellationToken cancellationToken = default)
    {
        await EnsureManageAsync(userId);

        if (request is null) throw new ValidationException("Dữ liệu không hợp lệ.");

        var code = string.IsNullOrWhiteSpace(request.Code) ? await NextCodeAsync(cancellationToken) : request.Code.Trim();
        if (await _context.ExportReceipts.AnyAsync(r => r.Code == code, cancellationToken))
            throw new ConflictException("Số phiếu đã tồn tại.");

        var receipt = new ExportReceipt
        {
            Code = code,
            WarehouseId = request.WarehouseId,
            CustomerId = request.CustomerId,
            Type = request.Type.Trim(),
            Status = ReceiptStatuses.Draft,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

        await ValidateHeaderAndLinesAsync(receipt, request.Lines, cancellationToken);

        _context.ExportReceipts.Add(receipt);
        await _context.SaveChangesAsync(cancellationToken);

        foreach (var line in request.Lines)
        {
            _context.ExportReceiptDetails.Add(new ExportReceiptDetail
            {
                ReceiptId = receipt.Id,
                ProductId = line.ProductId,
                Quantity = line.Quantity,
                Price = line.Price,
                LocationId = line.LocationId
            });
        }

        await _context.SaveChangesAsync(cancellationToken);

        var tracked = await _context.ExportReceipts.FirstAsync(r => r.Id == receipt.Id, cancellationToken);
        return await MapAsync(tracked, cancellationToken);
    }

    public async Task<ExportReceiptResponseDto> UpdateAsync(int userId, int id, UpdateExportReceiptRequestDto request, CancellationToken cancellationToken = default)
    {
        await EnsureManageAsync(userId);

        if (request is null) throw new ValidationException("Dữ liệu không hợp lệ.");

        var receipt = await _context.ExportReceipts
            .Include(r => r.ExportReceiptDetails)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (receipt is null) throw new NotFoundException("Không tìm thấy phiếu xuất.");
        if (receipt.Status != ReceiptStatuses.Draft)
            throw new ValidationException("Chỉ phiếu nháp mới được sửa.");

        receipt.Type = request.Type.Trim();
        receipt.CustomerId = request.CustomerId;
        receipt.UpdatedAt = DateTime.UtcNow;
        receipt.UpdatedBy = userId;

        await ValidateHeaderAndLinesAsync(receipt, request.Lines, cancellationToken);

        _context.ExportReceiptDetails.RemoveRange(receipt.ExportReceiptDetails);
        await _context.SaveChangesAsync(cancellationToken);

        foreach (var line in request.Lines)
        {
            _context.ExportReceiptDetails.Add(new ExportReceiptDetail
            {
                ReceiptId = receipt.Id,
                ProductId = line.ProductId,
                Quantity = line.Quantity,
                Price = line.Price,
                LocationId = line.LocationId
            });
        }

        await _context.SaveChangesAsync(cancellationToken);

        return await MapAsync(receipt, cancellationToken);
    }

    public async Task<ExportReceiptResponseDto> ConfirmAsync(int userId, int id, CancellationToken cancellationToken = default)
    {
        await EnsureManageAsync(userId);

        await using var tx = await _context.Database.BeginTransactionAsync(cancellationToken);

        var receipt = await _context.ExportReceipts
            .Include(r => r.ExportReceiptDetails)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (receipt is null) throw new NotFoundException("Không tìm thấy phiếu xuất.");
        if (receipt.Status != ReceiptStatuses.Draft)
            throw new ValidationException("Phiếu không ở trạng thái nháp.");

        foreach (var d in receipt.ExportReceiptDetails)
        {
            var qty = d.Quantity ?? 0;
            if (qty <= 0) throw new ValidationException("Số lượng dòng không hợp lệ.");

            await _stockService.ApplyInventoryDeltaAsync(
                receipt.WarehouseId,
                d.ProductId,
                d.LocationId,
                -qty,
                StockReferenceTypes.ExportReceipt,
                receipt.Id,
                userId,
                cancellationToken);
        }

        receipt.Status = ReceiptStatuses.Confirmed;
        receipt.ConfirmedAt = DateTime.UtcNow;
        receipt.ConfirmedBy = userId;

        await _context.SaveChangesAsync(cancellationToken);

        // Tạo notification trong cùng transaction để không bị lệch dữ liệu.
        await _notificationService.CreateForUserAsync(userId, new CreateNotificationRequestDto
        {
            Title = "Phiếu xuất đã được xác nhận",
            Content = $"Phiếu xuất ID={receipt.Id} (Code={receipt.Code}) đã được xác nhận thành công.",
            Type = "ExportReceiptConfirmed",
            ReferenceId = receipt.Id,
            ReferenceType = "ExportReceipt"
        });

        await tx.CommitAsync(cancellationToken);

        return await GetByIdAsync(userId, id, cancellationToken);
    }

    public async Task CancelAsync(int userId, int id, CancellationToken cancellationToken = default)
    {
        await EnsureManageAsync(userId);

        var receipt = await _context.ExportReceipts.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        if (receipt is null) throw new NotFoundException("Không tìm thấy phiếu xuất.");
        if (receipt.Status != ReceiptStatuses.Draft)
            throw new ValidationException("Chỉ hủy được phiếu nháp.");

        receipt.Status = ReceiptStatuses.Cancelled;
        receipt.CancelledAt = DateTime.UtcNow;
        receipt.CancelledBy = userId;
        await _context.SaveChangesAsync(cancellationToken);

        await _notificationService.CreateForUserAsync(userId, new CreateNotificationRequestDto
        {
            Title = "Phiếu xuất đã bị hủy",
            Content = $"Phiếu xuất ID={receipt.Id} (Code={receipt.Code}) đã được hủy.",
            Type = "ExportReceiptCancelled",
            ReferenceId = receipt.Id,
            ReferenceType = "ExportReceipt"
        });
    }
}
