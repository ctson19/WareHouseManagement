using WareManagement.DTO.PartnerDTO;
using WareManagement.Helpers;
using WareManagement.Models;
using WareManagement.Repository.Interfaces;
using WareManagement.Service.Exceptions;
using WareManagement.Service.Interfaces;

namespace WareManagement.Service.Implementations;

public class PartnerService : IPartnerService
{
    private readonly IPartnerRepository _partnerRepository;
    private readonly IUserRepository _userRepository;

    public PartnerService(IPartnerRepository partnerRepository, IUserRepository userRepository)
    {
        _partnerRepository = partnerRepository;
        _userRepository = userRepository;
    }

    private async Task EnsureReadAsync(int userId)
    {
        if (!await _userRepository.CanReadWarehouseDataAsync(userId))
            throw new ForbiddenException("Bạn không có quyền xem.");
    }

    private async Task EnsureManageAsync(int userId)
    {
        if (!await _userRepository.CanManageCatalogAsync(userId))
            throw new ForbiddenException("Bạn không có quyền quản lý đối tác.");
    }

    private static PartnerResponseDto Map(Partner p) => new()
    {
        Id = p.Id,
        Code = p.Code,
        Name = p.Name,
        Type = p.Type,
        Address = p.Address,
        Phone = p.Phone,
        Email = p.Email,
        TaxCode = p.TaxCode
    };

    public async Task<(List<PartnerResponseDto> Items, int Total)> GetPagedAsync(int userId, string? type, string? search, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        await EnsureReadAsync(userId);
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 200) pageSize = 200;

        var (items, total) = await _partnerRepository.GetPagedAsync(type, search, page, pageSize, cancellationToken);
        return (items.Select(Map).ToList(), total);
    }

    public async Task<PartnerResponseDto> GetByIdAsync(int userId, int id, CancellationToken cancellationToken = default)
    {
        await EnsureReadAsync(userId);
        var p = await _partnerRepository.GetByIdAsync(id, cancellationToken);
        if (p is null) throw new NotFoundException("Không tìm thấy đối tác.");

        return Map(p);
    }

    public async Task<PartnerResponseDto> CreateAsync(int userId, CreatePartnerRequestDto request, CancellationToken cancellationToken = default)
    {
        await EnsureManageAsync(userId);

        if (request is null) throw new ValidationException("Dữ liệu không hợp lệ.");
        if (string.IsNullOrWhiteSpace(request.Type)) throw new ValidationException("Loại đối tác là bắt buộc.");

        var t = request.Type.Trim();
        if (t != PartnerTypes.Supplier && t != PartnerTypes.Customer)
            throw new ValidationException("Loại đối tác phải là Supplier hoặc Customer.");

        if (string.IsNullOrWhiteSpace(request.Code)) throw new ValidationException("Mã đối tác là bắt buộc.");

        var code = request.Code.Trim();
        if (await _partnerRepository.CodeExistsForTypeAsync(code, t, null, cancellationToken))
            throw new ConflictException("Mã đối tác đã tồn tại trong loại này.");

        var entity = new Partner
        {
            Code = code,
            Name = string.IsNullOrWhiteSpace(request.Name) ? null : request.Name.Trim(),
            Type = t,
            Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim(),
            Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim(),
            Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
            TaxCode = string.IsNullOrWhiteSpace(request.TaxCode) ? null : request.TaxCode.Trim(),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

        var created = await _partnerRepository.AddAsync(entity, cancellationToken);
        return Map(created);
    }

    public async Task<PartnerResponseDto> UpdateAsync(int userId, int id, UpdatePartnerRequestDto request, CancellationToken cancellationToken = default)
    {
        await EnsureManageAsync(userId);

        if (request is null) throw new ValidationException("Dữ liệu không hợp lệ.");

        var p = await _partnerRepository.GetByIdAsync(id, cancellationToken);
        if (p is null) throw new NotFoundException("Không tìm thấy đối tác.");

        p.Name = string.IsNullOrWhiteSpace(request.Name) ? null : request.Name.Trim();
        p.Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim();
        p.Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim();
        p.Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();
        p.TaxCode = string.IsNullOrWhiteSpace(request.TaxCode) ? null : request.TaxCode.Trim();

        await _partnerRepository.UpdateAsync(p, cancellationToken);
        return Map(p);
    }

    public async Task DeleteAsync(int userId, int id, CancellationToken cancellationToken = default)
    {
        await EnsureManageAsync(userId);

        var p = await _partnerRepository.GetByIdAsync(id, cancellationToken);
        if (p is null) throw new NotFoundException("Không tìm thấy đối tác.");

        if (await _partnerRepository.HasReferencesAsync(id, cancellationToken))
            throw new ValidationException("Đối tác đã phát sinh phiếu — không xóa được.");

        await _partnerRepository.DeleteAsync(p, cancellationToken);
    }
}
