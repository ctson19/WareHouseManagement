using WareManagement.DTO.ProductDTO;
using WareManagement.DTO.UnitDTO;

namespace WareManagement.Service.Interfaces;

public interface IProductService
{
    Task<List<UnitResponseDto>> GetUnitsAsync(int userId, CancellationToken cancellationToken = default);
    Task<(List<ProductResponseDto> Items, int Total)> GetPagedAsync(int userId, string? search, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<ProductResponseDto> GetByIdAsync(int userId, int id, CancellationToken cancellationToken = default);
    Task<ProductResponseDto> CreateAsync(int userId, CreateProductRequestDto request, CancellationToken cancellationToken = default);
    Task<ProductResponseDto> UpdateAsync(int userId, int id, UpdateProductRequestDto request, CancellationToken cancellationToken = default);
}
