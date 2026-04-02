using WareManagement.DTO.InventoryDTO;

namespace WareManagement.Service.Interfaces;

public interface IInventoryService
{
    Task<List<InventorySummaryDto>> GetSummaryAsync(int userId, int? warehouseId, string? productCode, string? productName, CancellationToken cancellationToken = default);
}
