using Microsoft.EntityFrameworkCore;
using WareManagement.Helpers;
using WareManagement.Models;
using WareManagement.Service.Exceptions;
using WareManagement.Service.Interfaces;

namespace WareManagement.Service.Implementations;

public class StockService : IStockService
{
    private readonly WareManagementContext _context;

    public StockService(WareManagementContext context)
    {
        _context = context;
    }

    public async Task ApplyInventoryDeltaAsync(
        int warehouseId,
        int productId,
        int locationId,
        decimal delta,
        string referenceType,
        int referenceId,
        int userId,
        CancellationToken cancellationToken = default)
    {
        if (delta == 0) return;

        var inv = await _context.Inventories
            .FirstOrDefaultAsync(
                i => i.WarehouseId == warehouseId && i.ProductId == productId && i.LocationId == locationId,
                cancellationToken);

        if (inv is null)
        {
            if (delta < 0)
                throw new ValidationException("Không đủ tồn kho tại vị trí đã chọn.");

            inv = new Inventory
            {
                WarehouseId = warehouseId,
                ProductId = productId,
                LocationId = locationId,
                Quantity = delta
            };
            _context.Inventories.Add(inv);
        }
        else
        {
            var q = inv.Quantity ?? 0;
            var next = q + delta;
            if (next < 0)
                throw new ValidationException("Không đủ tồn kho để xuất (không cho phép âm kho).");

            inv.Quantity = next;
        }

        var txType = delta > 0 ? StockTransactionTypes.In : StockTransactionTypes.Out;
        var abs = Math.Abs(delta);

        _context.StockTransactions.Add(new StockTransaction
        {
            WarehouseId = warehouseId,
            ProductId = productId,
            LocationId = locationId,
            Quantity = abs,
            TransactionType = txType,
            ReferenceType = referenceType,
            ReferenceId = referenceId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        });
        // Caller gọi SaveChanges (thường trong một transaction).
    }
}
