using Microsoft.EntityFrameworkCore;
using WareManagement.DTO.InventoryDTO;
using WareManagement.Models;
using WareManagement.Repository.Interfaces;
using WareManagement.Service.Exceptions;
using WareManagement.Service.Interfaces;

namespace WareManagement.Service.Implementations;

public class InventoryService : IInventoryService
{
    private readonly WareManagementContext _context;
    private readonly IUserRepository _userRepository;

    public InventoryService(WareManagementContext context, IUserRepository userRepository)
    {
        _context = context;
        _userRepository = userRepository;
    }

    public async Task<List<InventorySummaryDto>> GetSummaryAsync(int userId, int? warehouseId, string? productCode, string? productName, CancellationToken cancellationToken = default)
    {
        if (!await _userRepository.CanReadWarehouseDataAsync(userId))
            throw new ForbiddenException("Bạn không có quyền xem tồn kho.");

        var q = _context.Inventories
            .AsNoTracking()
            .Include(i => i.Product)
            .Include(i => i.Warehouse)
            .AsQueryable();

        if (warehouseId.HasValue)
            q = q.Where(i => i.WarehouseId == warehouseId.Value);

        if (!string.IsNullOrWhiteSpace(productCode))
        {
            var c = productCode.Trim();
            q = q.Where(i => i.Product != null && i.Product.Code != null && i.Product.Code.Contains(c));
        }

        if (!string.IsNullOrWhiteSpace(productName))
        {
            var n = productName.Trim();
            q = q.Where(i => i.Product != null && i.Product.Name != null && i.Product.Name.Contains(n));
        }

        var rows = await q.ToListAsync(cancellationToken);

        var grouped = rows
            .GroupBy(i => new { i.WarehouseId, i.ProductId })
            .Select(g =>
            {
                var first = g.First();
                var total = g.Sum(x => x.Quantity ?? 0);
                var min = first.Product?.MinStock ?? 0;
                var max = first.Product?.MaxStock ?? 0;

                string alert = "Ok";
                if (total < min) alert = "BelowMin";
                else if (max > 0 && total > max) alert = "AboveMax";

                return new InventorySummaryDto
                {
                    WarehouseId = g.Key.WarehouseId,
                    WarehouseCode = first.Warehouse?.Code ?? "",
                    ProductId = g.Key.ProductId,
                    ProductCode = first.Product?.Code ?? "",
                    ProductName = first.Product?.Name,
                    TotalQuantity = total,
                    MinStock = first.Product?.MinStock,
                    MaxStock = first.Product?.MaxStock,
                    StockAlert = alert
                };
            })
            .OrderBy(x => x.WarehouseCode)
            .ThenBy(x => x.ProductCode)
            .ToList();

        return grouped;
    }
}
