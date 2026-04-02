using WareManagement.DTO.LocationDTO;
using WareManagement.Helpers;
using WareManagement.Models;
using WareManagement.Repository.Interfaces;
using WareManagement.Service.Exceptions;
using WareManagement.Service.Interfaces;

namespace WareManagement.Service.Implementations;

public class LocationService : ILocationService
{
    private readonly ILocationRepository _locationRepository;
    private readonly IWareHouseRepository _warehouseRepository;
    private readonly IUserRepository _userRepository;

    public LocationService(
        ILocationRepository locationRepository,
        IWareHouseRepository warehouseRepository,
        IUserRepository userRepository)
    {
        _locationRepository = locationRepository;
        _warehouseRepository = warehouseRepository;
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
            throw new ForbiddenException("Bạn không có quyền quản lý vị trí.");
    }

    private static LocationResponseDto Map(Location l) => new()
    {
        Id = l.Id,
        WarehouseId = l.WarehouseId,
        ParentId = l.ParentId,
        Name = l.Name,
        Type = l.Type
    };

    public async Task<List<LocationResponseDto>> GetByWarehouseAsync(int userId, int warehouseId, CancellationToken cancellationToken = default)
    {
        await EnsureReadAsync(userId);
        var wh = await _warehouseRepository.GetByIdAsync(warehouseId, cancellationToken);
        if (wh is null) throw new NotFoundException("Không tìm thấy kho.");

        var list = await _locationRepository.GetByWarehouseAsync(warehouseId, cancellationToken);
        return list.Select(Map).ToList();
    }

    public async Task<LocationResponseDto> CreateAsync(int userId, CreateLocationRequestDto request, CancellationToken cancellationToken = default)
    {
        await EnsureManageAsync(userId);

        if (request is null) throw new ValidationException("Dữ liệu không hợp lệ.");
        if (string.IsNullOrWhiteSpace(request.Name)) throw new ValidationException("Tên vị trí là bắt buộc.");
        if (string.IsNullOrWhiteSpace(request.Type)) throw new ValidationException("Loại vị trí là bắt buộc.");

        var wh = await _warehouseRepository.GetByIdAsync(request.WarehouseId, cancellationToken);
        if (wh is null) throw new NotFoundException("Không tìm thấy kho.");

        if (request.ParentId.HasValue)
        {
            var parent = await _locationRepository.GetByIdAsync(request.ParentId.Value, cancellationToken);
            if (parent is null || parent.WarehouseId != request.WarehouseId)
                throw new ValidationException("Vị trí cha không hợp lệ.");
        }

        var entity = new Location
        {
            WarehouseId = request.WarehouseId,
            ParentId = request.ParentId,
            Name = request.Name.Trim(),
            Type = request.Type.Trim()
        };

        var created = await _locationRepository.AddAsync(entity, cancellationToken);
        return Map(created);
    }

    public async Task<LocationResponseDto> UpdateAsync(int userId, int id, UpdateLocationRequestDto request, CancellationToken cancellationToken = default)
    {
        await EnsureManageAsync(userId);

        if (request is null) throw new ValidationException("Dữ liệu không hợp lệ.");
        if (string.IsNullOrWhiteSpace(request.Name)) throw new ValidationException("Tên vị trí là bắt buộc.");

        var loc = await _locationRepository.GetByIdAsync(id, cancellationToken);
        if (loc is null) throw new NotFoundException("Không tìm thấy vị trí.");

        if (request.ParentId.HasValue)
        {
            if (request.ParentId.Value == id)
                throw new ValidationException("Không thể đặt chính mình làm cha.");

            var parent = await _locationRepository.GetByIdAsync(request.ParentId.Value, cancellationToken);
            if (parent is null || parent.WarehouseId != loc.WarehouseId)
                throw new ValidationException("Vị trí cha không hợp lệ.");
        }

        loc.ParentId = request.ParentId;
        loc.Name = request.Name.Trim();

        await _locationRepository.UpdateAsync(loc, cancellationToken);
        return Map(loc);
    }

    public async Task DeleteAsync(int userId, int id, CancellationToken cancellationToken = default)
    {
        await EnsureManageAsync(userId);

        var loc = await _locationRepository.GetByIdAsync(id, cancellationToken);
        if (loc is null) throw new NotFoundException("Không tìm thấy vị trí.");

        if (await _locationRepository.HasChildrenAsync(id, cancellationToken))
            throw new ValidationException("Có vị trí con — không xóa được.");

        await _locationRepository.DeleteAsync(loc, cancellationToken);
    }
}
