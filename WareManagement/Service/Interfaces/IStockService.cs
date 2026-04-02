namespace WareManagement.Service.Interfaces;

/// <summary>Cập nhật tồn theo vị trí và ghi StockTransaction.</summary>
public interface IStockService
{
    /// <param name="delta">Dương = nhập, âm = xuất.</param>
    Task ApplyInventoryDeltaAsync(
        int warehouseId,
        int productId,
        int locationId,
        decimal delta,
        string referenceType,
        int referenceId,
        int userId,
        CancellationToken cancellationToken = default);
}
