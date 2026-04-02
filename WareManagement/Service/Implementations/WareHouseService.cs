using WareManagement.DTO.WarehouseDTO;
using WareManagement.Models;
using WareManagement.Repository.Interfaces;
using WareManagement.Service.Exceptions;
using WareManagement.Service.Interfaces;

namespace WareManagement.Service.Implementations;

public class WareHouseService : IWareHouseService
{
    private readonly IWareHouseRepository _warehouseRepository;
    private readonly IUserRepository _userRepository;

    public WareHouseService(IWareHouseRepository warehouseRepository, IUserRepository userRepository)
    {
        _warehouseRepository = warehouseRepository;
        _userRepository = userRepository;
    }

    private async Task EnsureReadAsync(int userId)
    {
        if (!await _userRepository.CanReadWarehouseDataAsync(userId))
            throw new ForbiddenException("Bạn không có quyền xem dữ liệu kho.");
    }

    private async Task EnsureManageAsync(int userId)
    {
        if (!await _userRepository.CanManageCatalogAsync(userId))
            throw new ForbiddenException("Bạn không có quyền quản lý danh mục kho.");
    }

    private static WarehouseResponseDto Map(Warehouse w) => new()
    {
        Id = w.Id,
        Code = w.Code,
        Name = w.Name,
        Address = w.Address,
        IsActive = w.Status == true
    };

    public async Task<List<WarehouseResponseDto>> GetAllAsync(int userId, CancellationToken cancellationToken = default)
    {
        await EnsureReadAsync(userId);
        var list = await _warehouseRepository.GetAllAsync(cancellationToken);
        return list.Select(Map).ToList();
    }

    public async Task<(List<WarehouseResponseDto> Items, int Total)> GetPagedAsync(int userId, string? search, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        await EnsureReadAsync(userId);
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 200) pageSize = 200;

        var (items, total) = await _warehouseRepository.GetPagedAsync(search, page, pageSize, cancellationToken);
        return (items.Select(Map).ToList(), total);
    }

    public async Task<WarehouseResponseDto> GetByIdAsync(int userId, int id, CancellationToken cancellationToken = default)
    {
        await EnsureReadAsync(userId);
        var w = await _warehouseRepository.GetByIdAsync(id, cancellationToken);
        if (w is null) throw new NotFoundException("Không tìm thấy kho.");

        return Map(w);
    }

    public async Task<WarehouseResponseDto> CreateAsync(int userId, CreateWarehouseRequestDto request, CancellationToken cancellationToken = default)
    {
        await EnsureManageAsync(userId);

        if (request is null) throw new ValidationException("Dữ liệu không hợp lệ.");
        if (string.IsNullOrWhiteSpace(request.Code)) throw new ValidationException("Mã kho là bắt buộc.");
        if (request.Code.Length > 50) throw new ValidationException("Mã kho quá dài.");

        var code = request.Code.Trim();
        if (await _warehouseRepository.CodeExistsAsync(code, null, cancellationToken))
            throw new ConflictException("Mã kho đã tồn tại.");

        var entity = new Warehouse
        {
            Code = code,
            Name = string.IsNullOrWhiteSpace(request.Name) ? null : request.Name.Trim(),
            Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim(),
            Status = request.IsActive,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

        var created = await _warehouseRepository.AddAsync(entity, cancellationToken);
        return Map(created);
    }

    public async Task<WarehouseResponseDto> UpdateAsync(int userId, int id, UpdateWarehouseRequestDto request, CancellationToken cancellationToken = default)
    {
        await EnsureManageAsync(userId);

        if (request is null) throw new ValidationException("Dữ liệu không hợp lệ.");

        var w = await _warehouseRepository.GetByIdAsync(id, cancellationToken);
        if (w is null) throw new NotFoundException("Không tìm thấy kho.");

        w.Name = string.IsNullOrWhiteSpace(request.Name) ? null : request.Name.Trim();
        w.Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim();
        w.Status = request.IsActive;
        w.UpdatedAt = DateTime.UtcNow;
        w.UpdatedBy = userId;

        await _warehouseRepository.UpdateAsync(w, cancellationToken);
        return Map(w);
    }
}
