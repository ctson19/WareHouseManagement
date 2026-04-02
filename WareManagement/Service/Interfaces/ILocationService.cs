using WareManagement.DTO.LocationDTO;

namespace WareManagement.Service.Interfaces;

public interface ILocationService
{
    Task<List<LocationResponseDto>> GetByWarehouseAsync(int userId, int warehouseId, CancellationToken cancellationToken = default);
    Task<LocationResponseDto> CreateAsync(int userId, CreateLocationRequestDto request, CancellationToken cancellationToken = default);
    Task<LocationResponseDto> UpdateAsync(int userId, int id, UpdateLocationRequestDto request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int userId, int id, CancellationToken cancellationToken = default);
}
