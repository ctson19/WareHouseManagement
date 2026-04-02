using System;
using WareManagement.Models;

namespace WareManagement.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByUsername(string username);
        Task<User?> GetByIdAsync(int id);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> IsAdminAsync(int userId);

        Task<bool> HasAnyRoleAsync(int userId, params string[] roleNames);

        /// <summary>Admin hoặc Thủ kho — CRUD danh mục, phiếu nhập/xuất/điều chuyển.</summary>
        Task<bool> CanManageCatalogAsync(int userId);

        /// <summary>Đọc tồn kho, báo cáo (Admin, Thủ kho, Kế toán kho, User xem).</summary>
        Task<bool> CanReadWarehouseDataAsync(int userId);

        Task<User> CreateUserAsync(string username, string passwordHash, bool isActive, DateTime utcNow);
        Task UpdatePasswordHashAsync(int userId, string passwordHash, DateTime utcNow);
        Task SoftDeleteAsync(int userId, DateTime utcNow);
        Task<User> UpdateIsActiveAsync(int userId, bool isActive, DateTime utcNow);
    }
}
