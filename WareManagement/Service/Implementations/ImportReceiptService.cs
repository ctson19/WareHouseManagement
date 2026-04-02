using Microsoft.EntityFrameworkCore;
using WareManagement.DTO.ImportReceiptDTO;
using WareManagement.DTO.NotificationDTO;
using WareManagement.Helpers;
using WareManagement.Models;
using WareManagement.Repository.Interfaces;
using WareManagement.Service.Exceptions;
using WareManagement.Service.Interfaces;

namespace WareManagement.Service.Implementations;

public class ImportReceiptService : IImportReceiptService
{
    private readonly WareManagementContext _context;
    private readonly IUserRepository _userRepository;
    private readonly IStockService _stockService;
    private readonly INotificationService _notificationService;

    public ImportReceiptService(
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
            throw new ForbiddenException("Bạn không có quyền quản lý phiếu nhập.");
    }

    private async Task EnsureReadAsync(int userId)
    {
        if (!await _userRepository.CanReadWarehouseDataAsync(userId))
            throw new ForbiddenException("Bạn không có quyền xem.");
    }

    private async Task<string> NextCodeAsync(CancellationToken cancellationToken)
    {
        var prefix = "PN" + DateTime.UtcNow.ToString("yyyyMMdd");
        var count = await _context.ImportReceipts.CountAsync(r => r.Code.StartsWith(prefix), cancellationToken);
        return $"{prefix}{(count + 1):D4}";
    }

    private static void ValidateLines(List<ImportReceiptLineDto> lines)
    {
        if (lines is null || lines.Count == 0)
            throw new ValidationException("Phiếu nhập phải có ít nhất một dòng hàng.");

        foreach (var line in lines)
        {
            if (line.Quantity <= 0) throw new ValidationException("Số lượng phải lớn hơn 0.");
            if (line.Price < 0) throw new ValidationException("Đơn giá không hợp lệ.");
        }
    }

    private static readonly string[] ValidImportTypes =
    {
        ImportReceiptTypes.Purchase,
        ImportReceiptTypes.TransferIn,
        ImportReceiptTypes.StockAdjustIncrease
    };

    private async Task ValidateHeaderAndLinesAsync(ImportReceipt receipt, List<ImportReceiptLineDto> lines, CancellationToken cancellationToken)
    {
        ValidateLines(lines);

        if (!ValidImportTypes.Contains(receipt.Type))
            throw new ValidationException("Loại phiếu nhập không hợp lệ.");

        if (receipt.Type == ImportReceiptTypes.Purchase && receipt.SupplierId is null)
            throw new ValidationException("Nhập mua phải chọn nhà cung cấp.");

        var wh = await _context.Warehouses.FirstOrDefaultAsync(w => w.Id == receipt.WarehouseId, cancellationToken);
        if (wh is null) throw new ValidationException("Kho không tồn tại.");

        foreach (var line in lines)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == line.ProductId, cancellationToken);
            if (product is null) throw new ValidationException($"Hàng Id={line.ProductId} không tồn tại.");

            var loc = await _context.Locations.FirstOrDefaultAsync(l => l.Id == line.LocationId, cancellationToken);
            if (loc is null || loc.WarehouseId != receipt.WarehouseId)
                throw new ValidationException($"Vị trí Id={line.LocationId} không thuộc kho nhập.");
        }

        if (receipt.SupplierId.HasValue)
        {
            var sup = await _context.Partners.FirstOrDefaultAsync(p => p.Id == receipt.SupplierId && p.Type == PartnerTypes.Supplier, cancellationToken);
            if (sup is null) throw new ValidationException("Nhà cung cấp không hợp lệ.");
        }
    }

    private async Task<ImportReceiptResponseDto> MapAsync(ImportReceipt r, CancellationToken cancellationToken)
    {
        await _context.Entry(r).Collection(x => x.ImportReceiptDetails).Query()
            .Include(d => d.Product)
            .Include(d => d.Location)
            .LoadAsync(cancellationToken);

        var wh = await _context.Warehouses.AsNoTracking().FirstOrDefaultAsync(w => w.Id == r.WarehouseId, cancellationToken);
        string? supName = null;
        if (r.SupplierId.HasValue)
        {
            var sup = await _context.Partners.AsNoTracking().FirstOrDefaultAsync(p => p.Id == r.SupplierId, cancellationToken);
            supName = sup?.Name;
        }

        var lines = r.ImportReceiptDetails.OrderBy(d => d.Id).Select(d => new ImportReceiptDetailResponseDto
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

        return new ImportReceiptResponseDto
        {
            Id = r.Id,
            Code = r.Code,
            WarehouseId = r.WarehouseId,
            WarehouseCode = wh?.Code,
            SupplierId = r.SupplierId,
            SupplierName = supName,
            Type = r.Type,
            Status = r.Status,
            TotalAmount = r.TotalAmount,
            CreatedAt = r.CreatedAt,
            Lines = lines
        };
    }

    public async Task<(List<ImportReceiptResponseDto> Items, int Total)> GetPagedAsync(int userId, int? warehouseId, string? status, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        await EnsureReadAsync(userId);
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 200) pageSize = 200;

        var q = _context.ImportReceipts.AsNoTracking().AsQueryable();
        if (warehouseId.HasValue) q = q.Where(r => r.WarehouseId == warehouseId.Value);
        if (!string.IsNullOrWhiteSpace(status)) q = q.Where(r => r.Status == status.Trim());

        var total = await q.CountAsync(cancellationToken);
        var ids = await q.OrderByDescending(r => r.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).Select(r => r.Id).ToListAsync(cancellationToken);

        var list = new List<ImportReceiptResponseDto>();
        foreach (var id in ids)
        {
            var entity = await _context.ImportReceipts.FirstAsync(r => r.Id == id, cancellationToken);
            list.Add(await MapAsync(entity, cancellationToken));
        }

        return (list, total);
    }

    public async Task<ImportReceiptResponseDto> GetByIdAsync(int userId, int id, CancellationToken cancellationToken = default)
    {
        await EnsureReadAsync(userId);
        var r = await _context.ImportReceipts.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (r is null) throw new NotFoundException("Không tìm thấy phiếu nhập.");

        return await MapAsync(r, cancellationToken);
    }

    public async Task<ImportReceiptResponseDto> CreateAsync(int userId, CreateImportReceiptRequestDto request, CancellationToken cancellationToken = default)
    {
        await EnsureManageAsync(userId);

        if (request is null) throw new ValidationException("Dữ liệu không hợp lệ.");

        var code = string.IsNullOrWhiteSpace(request.Code) ? await NextCodeAsync(cancellationToken) : request.Code.Trim();
        if (await _context.ImportReceipts.AnyAsync(r => r.Code == code, cancellationToken))
            throw new ConflictException("Số phiếu đã tồn tại.");

        var receipt = new ImportReceipt
        {
            Code = code,
            WarehouseId = request.WarehouseId,
            SupplierId = request.SupplierId,
            Type = request.Type.Trim(),
            Status = ReceiptStatuses.Draft,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

        await ValidateHeaderAndLinesAsync(receipt, request.Lines, cancellationToken);

        receipt.TotalAmount = request.Lines.Sum(l => l.Quantity * l.Price);

        _context.ImportReceipts.Add(receipt);
        await _context.SaveChangesAsync(cancellationToken);

        foreach (var line in request.Lines)
        {
            _context.ImportReceiptDetails.Add(new ImportReceiptDetail
            {
                ReceiptId = receipt.Id,
                ProductId = line.ProductId,
                Quantity = line.Quantity,
                Price = line.Price,
                LocationId = line.LocationId
            });
        }

        await _context.SaveChangesAsync(cancellationToken);

        var tracked = await _context.ImportReceipts.FirstAsync(r => r.Id == receipt.Id, cancellationToken);
        return await MapAsync(tracked, cancellationToken);
    }

    public async Task<ImportReceiptResponseDto> UpdateAsync(int userId, int id, UpdateImportReceiptRequestDto request, CancellationToken cancellationToken = default)
    {
        await EnsureManageAsync(userId);

        if (request is null) throw new ValidationException("Dữ liệu không hợp lệ.");

        var receipt = await _context.ImportReceipts
            .Include(r => r.ImportReceiptDetails)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (receipt is null) throw new NotFoundException("Không tìm thấy phiếu nhập.");
        if (receipt.Status != ReceiptStatuses.Draft)
            throw new ValidationException("Chỉ phiếu nháp mới được sửa.");

        receipt.Type = request.Type.Trim();
        receipt.SupplierId = request.SupplierId;
        receipt.UpdatedAt = DateTime.UtcNow;
        receipt.UpdatedBy = userId;

        await ValidateHeaderAndLinesAsync(receipt, request.Lines, cancellationToken);

        _context.ImportReceiptDetails.RemoveRange(receipt.ImportReceiptDetails);
        await _context.SaveChangesAsync(cancellationToken);

        foreach (var line in request.Lines)
        {
            _context.ImportReceiptDetails.Add(new ImportReceiptDetail
            {
                ReceiptId = receipt.Id,
                ProductId = line.ProductId,
                Quantity = line.Quantity,
                Price = line.Price,
                LocationId = line.LocationId
            });
        }

        receipt.TotalAmount = request.Lines.Sum(l => l.Quantity * l.Price);
        await _context.SaveChangesAsync(cancellationToken);

        return await MapAsync(receipt, cancellationToken);
    }

    public async Task<ImportReceiptResponseDto> ConfirmAsync(int userId, int id, CancellationToken cancellationToken = default)
    {
        await EnsureManageAsync(userId);

        await using var tx = await _context.Database.BeginTransactionAsync(cancellationToken);

        var receipt = await _context.ImportReceipts
            .Include(r => r.ImportReceiptDetails)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (receipt is null) throw new NotFoundException("Không tìm thấy phiếu nhập.");
        if (receipt.Status != ReceiptStatuses.Draft)
            throw new ValidationException("Phiếu không ở trạng thái nháp.");

        foreach (var d in receipt.ImportReceiptDetails)
        {
            var qty = d.Quantity ?? 0;
            if (qty <= 0) throw new ValidationException("Số lượng dòng không hợp lệ.");

            await _stockService.ApplyInventoryDeltaAsync(
                receipt.WarehouseId,
                d.ProductId,
                d.LocationId,
                qty,
                StockReferenceTypes.ImportReceipt,
                receipt.Id,
                userId,
                cancellationToken);
        }

        receipt.Status = ReceiptStatuses.Confirmed;
        receipt.ConfirmedAt = DateTime.UtcNow;
        receipt.ConfirmedBy = userId;
        receipt.TotalAmount = receipt.ImportReceiptDetails.Sum(x => (x.Quantity ?? 0) * (x.Price ?? 0));

        await _context.SaveChangesAsync(cancellationToken);

        await _notificationService.CreateForUserAsync(userId, new CreateNotificationRequestDto
        {
            Title = "Phiếu nhập đã được xác nhận",
            Content = $"Phiếu nhập ID={receipt.Id} (Code={receipt.Code}) đã được xác nhận thành công.",
            Type = "ImportReceiptConfirmed",
            ReferenceId = receipt.Id,
            ReferenceType = "ImportReceipt"
        });

        await tx.CommitAsync(cancellationToken);

        return await GetByIdAsync(userId, id, cancellationToken);
    }

    public async Task CancelAsync(int userId, int id, CancellationToken cancellationToken = default)
    {
        await EnsureManageAsync(userId);

        var receipt = await _context.ImportReceipts.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        if (receipt is null) throw new NotFoundException("Không tìm thấy phiếu nhập.");
        if (receipt.Status != ReceiptStatuses.Draft)
            throw new ValidationException("Chỉ hủy được phiếu nháp.");

        receipt.Status = ReceiptStatuses.Cancelled;
        receipt.CancelledAt = DateTime.UtcNow;
        receipt.CancelledBy = userId;
        await _context.SaveChangesAsync(cancellationToken);

        await _notificationService.CreateForUserAsync(userId, new CreateNotificationRequestDto
        {
            Title = "Phiếu nhập đã bị hủy",
            Content = $"Phiếu nhập ID={receipt.Id} (Code={receipt.Code}) đã được hủy.",
            Type = "ImportReceiptCancelled",
            ReferenceId = receipt.Id,
            ReferenceType = "ImportReceipt"
        });
    }
}
