using WareManagement.DTO.ProductDTO;
using WareManagement.DTO.UnitDTO;
using WareManagement.Models;
using WareManagement.Repository.Interfaces;
using WareManagement.Service.Exceptions;
using WareManagement.Service.Interfaces;

namespace WareManagement.Service.Implementations;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IUserRepository _userRepository;

    public ProductService(IProductRepository productRepository, IUserRepository userRepository)
    {
        _productRepository = productRepository;
        _userRepository = userRepository;
    }

    private async Task EnsureReadAsync(int userId)
    {
        if (!await _userRepository.CanReadWarehouseDataAsync(userId))
            throw new ForbiddenException("Bạn không có quyền xem dữ liệu.");
    }

    private async Task EnsureManageAsync(int userId)
    {
        if (!await _userRepository.CanManageCatalogAsync(userId))
            throw new ForbiddenException("Bạn không có quyền quản lý hàng hóa.");
    }

    private static ProductResponseDto Map(Product p) => new()
    {
        Id = p.Id,
        Code = p.Code,
        Name = p.Name,
        UnitId = p.UnitId,
        UnitName = p.Unit?.Name,
        Barcode = p.Barcode,
        ImportPrice = p.ImportPrice,
        SalePrice = p.SalePrice,
        MinStock = p.MinStock,
        MaxStock = p.MaxStock,
        IsActive = p.IsActive != false
    };

    public async Task<List<UnitResponseDto>> GetUnitsAsync(int userId, CancellationToken cancellationToken = default)
    {
        await EnsureReadAsync(userId);
        var units = await _productRepository.GetUnitsAsync(cancellationToken);
        return units.Select(u => new UnitResponseDto { Id = u.Id, Name = u.Name }).ToList();
    }

    public async Task<(List<ProductResponseDto> Items, int Total)> GetPagedAsync(int userId, string? search, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        await EnsureReadAsync(userId);
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 200) pageSize = 200;

        var (items, total) = await _productRepository.GetPagedAsync(search, page, pageSize, cancellationToken);
        return (items.Select(Map).ToList(), total);
    }

    public async Task<ProductResponseDto> GetByIdAsync(int userId, int id, CancellationToken cancellationToken = default)
    {
        await EnsureReadAsync(userId);
        var p = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (p is null) throw new NotFoundException("Không tìm thấy hàng hóa.");

        return Map(p);
    }

    public async Task<ProductResponseDto> CreateAsync(int userId, CreateProductRequestDto request, CancellationToken cancellationToken = default)
    {
        await EnsureManageAsync(userId);

        if (request is null) throw new ValidationException("Dữ liệu không hợp lệ.");
        if (string.IsNullOrWhiteSpace(request.Code)) throw new ValidationException("Mã hàng là bắt buộc.");

        var code = request.Code.Trim();
        if (await _productRepository.CodeExistsAsync(code, null, cancellationToken))
            throw new ConflictException("Mã hàng đã tồn tại.");

        var unit = await _productRepository.GetUnitByIdAsync(request.UnitId, cancellationToken);
        if (unit is null) throw new ValidationException("Đơn vị tính không hợp lệ.");

        var entity = new Product
        {
            Code = code,
            Name = string.IsNullOrWhiteSpace(request.Name) ? null : request.Name.Trim(),
            UnitId = request.UnitId,
            Barcode = string.IsNullOrWhiteSpace(request.Barcode) ? null : request.Barcode.Trim(),
            ImportPrice = request.ImportPrice,
            SalePrice = request.SalePrice,
            MinStock = request.MinStock ?? 0,
            MaxStock = request.MaxStock ?? 0,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

        var created = await _productRepository.AddAsync(entity, cancellationToken);
        created.Unit = unit;
        return Map(created);
    }

    public async Task<ProductResponseDto> UpdateAsync(int userId, int id, UpdateProductRequestDto request, CancellationToken cancellationToken = default)
    {
        await EnsureManageAsync(userId);

        if (request is null) throw new ValidationException("Dữ liệu không hợp lệ.");

        var p = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (p is null) throw new NotFoundException("Không tìm thấy hàng hóa.");

        var unit = await _productRepository.GetUnitByIdAsync(request.UnitId, cancellationToken);
        if (unit is null) throw new ValidationException("Đơn vị tính không hợp lệ.");

        p.Name = string.IsNullOrWhiteSpace(request.Name) ? null : request.Name.Trim();
        p.UnitId = request.UnitId;
        p.Barcode = string.IsNullOrWhiteSpace(request.Barcode) ? null : request.Barcode.Trim();
        p.ImportPrice = request.ImportPrice;
        p.SalePrice = request.SalePrice;
        p.MinStock = request.MinStock ?? 0;
        p.MaxStock = request.MaxStock ?? 0;
        p.IsActive = request.IsActive;
        p.UpdatedAt = DateTime.UtcNow;
        p.UpdatedBy = userId;

        await _productRepository.UpdateAsync(p, cancellationToken);
        p.Unit = unit;
        return Map(p);
    }
}
