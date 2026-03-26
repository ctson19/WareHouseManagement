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

        Task<User> CreateUserAsync(string username, string passwordHash, bool isActive, DateTime utcNow);
        Task UpdatePasswordHashAsync(int userId, string passwordHash, DateTime utcNow);
        Task SoftDeleteAsync(int userId, DateTime utcNow);
        Task<User> UpdateIsActiveAsync(int userId, bool isActive, DateTime utcNow);
    }
}
