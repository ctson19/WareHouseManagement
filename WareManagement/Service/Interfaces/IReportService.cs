using WareManagement.DTO.InventoryDTO;
using WareManagement.DTO.ReportDTO;

namespace WareManagement.Service.Interfaces;

public interface IReportService
{
    Task<DashboardSummaryDto> GetDashboardAsync(int userId, CancellationToken cancellationToken = default);
    Task<List<NxtRowDto>> GetNxtAsync(int userId, DateTime fromUtc, DateTime toUtc, int? warehouseId, CancellationToken cancellationToken = default);
    Task<List<StockTransactionRowDto>> GetProductTransactionHistoryAsync(int userId, int productId, int? warehouseId, CancellationToken cancellationToken = default);
    Task<List<InventorySummaryDto>> GetLowStockAsync(int userId, int? warehouseId, CancellationToken cancellationToken = default);
    Task<List<PartnerReceiptRowDto>> GetReceiptsByPartnerAsync(int userId, int partnerId, CancellationToken cancellationToken = default);
}
