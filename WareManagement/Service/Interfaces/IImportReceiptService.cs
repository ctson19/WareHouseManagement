using WareManagement.DTO.ImportReceiptDTO;

namespace WareManagement.Service.Interfaces;

public interface IImportReceiptService
{
    Task<(List<ImportReceiptResponseDto> Items, int Total)> GetPagedAsync(int userId, int? warehouseId, string? status, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<ImportReceiptResponseDto> GetByIdAsync(int userId, int id, CancellationToken cancellationToken = default);
    Task<ImportReceiptResponseDto> CreateAsync(int userId, CreateImportReceiptRequestDto request, CancellationToken cancellationToken = default);
    Task<ImportReceiptResponseDto> UpdateAsync(int userId, int id, UpdateImportReceiptRequestDto request, CancellationToken cancellationToken = default);
    Task<ImportReceiptResponseDto> ConfirmAsync(int userId, int id, CancellationToken cancellationToken = default);
    Task CancelAsync(int userId, int id, CancellationToken cancellationToken = default);
}
