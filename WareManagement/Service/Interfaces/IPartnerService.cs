using WareManagement.DTO.PartnerDTO;

namespace WareManagement.Service.Interfaces;

public interface IPartnerService
{
    Task<(List<PartnerResponseDto> Items, int Total)> GetPagedAsync(int userId, string? type, string? search, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<PartnerResponseDto> GetByIdAsync(int userId, int id, CancellationToken cancellationToken = default);
    Task<PartnerResponseDto> CreateAsync(int userId, CreatePartnerRequestDto request, CancellationToken cancellationToken = default);
    Task<PartnerResponseDto> UpdateAsync(int userId, int id, UpdatePartnerRequestDto request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int userId, int id, CancellationToken cancellationToken = default);
}
