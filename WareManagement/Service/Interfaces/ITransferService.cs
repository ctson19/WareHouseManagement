using WareManagement.DTO.TransferDTO;

namespace WareManagement.Service.Interfaces;

public interface ITransferService
{
    Task<(List<TransferResponseDto> Items, int Total)> GetPagedAsync(int userId, string? status, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<TransferResponseDto> GetByIdAsync(int userId, int id, CancellationToken cancellationToken = default);
    Task<TransferResponseDto> CreateAsync(int userId, CreateTransferRequestDto request, CancellationToken cancellationToken = default);
    Task<TransferResponseDto> ConfirmAsync(int userId, int id, TransferConfirmRequestDto request, CancellationToken cancellationToken = default);
    Task SetStatusAsync(int userId, int id, string status, CancellationToken cancellationToken = default);
}
