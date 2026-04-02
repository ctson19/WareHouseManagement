using WareManagement.DTO.ExportReceiptDTO;

namespace WareManagement.Service.Interfaces;

public interface IExportReceiptService
{
    Task<(List<ExportReceiptResponseDto> Items, int Total)> GetPagedAsync(int userId, int? warehouseId, string? status, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<ExportReceiptResponseDto> GetByIdAsync(int userId, int id, CancellationToken cancellationToken = default);
    Task<ExportReceiptResponseDto> CreateAsync(int userId, CreateExportReceiptRequestDto request, CancellationToken cancellationToken = default);
    Task<ExportReceiptResponseDto> UpdateAsync(int userId, int id, UpdateExportReceiptRequestDto request, CancellationToken cancellationToken = default);
    Task<ExportReceiptResponseDto> ConfirmAsync(int userId, int id, CancellationToken cancellationToken = default);
    Task CancelAsync(int userId, int id, CancellationToken cancellationToken = default);
}
