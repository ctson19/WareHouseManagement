using WareManagement.DTO.WarehouseDTO;

namespace WareManagement.Service.Interfaces;

public interface IWareHouseService
{
    Task<List<WarehouseResponseDto>> GetAllAsync(int userId, CancellationToken cancellationToken = default);
    Task<(List<WarehouseResponseDto> Items, int Total)> GetPagedAsync(int userId, string? search, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<WarehouseResponseDto> GetByIdAsync(int userId, int id, CancellationToken cancellationToken = default);
    Task<WarehouseResponseDto> CreateAsync(int userId, CreateWarehouseRequestDto request, CancellationToken cancellationToken = default);
    Task<WarehouseResponseDto> UpdateAsync(int userId, int id, UpdateWarehouseRequestDto request, CancellationToken cancellationToken = default);
}
