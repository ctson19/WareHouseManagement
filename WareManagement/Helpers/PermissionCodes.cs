namespace WareManagement.Helpers;

public static class PermissionCodes
{
    // CRUD danh mục + các nghiệp vụ phiếu nhập/xuất/điều chuyển.
    public const string ManageCatalog = "MANAGE_CATALOG";

    // Đọc tồn kho, báo cáo.
    public const string ReadWarehouseData = "READ_WAREHOUSE_DATA";

    // (Tùy chọn) admin thao tác quản trị hệ thống.
    public const string Admin = "ADMIN";
}
