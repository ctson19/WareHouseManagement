using System;
using WareManagement.Models;
using WareManagement.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace WareManagement.Repository.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly WareManagementContext _context;

        public UserRepository(WareManagementContext context)
        {
            _context = context;
        }

        public Task<User?> GetByUsername(string username)
        {
            return _context.Users
                .FirstOrDefaultAsync(x => x.Username == username && x.IsActive == true);
        }

        public Task<User?> GetByIdAsync(int id)
        {
            return _context.Users
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<bool> UsernameExistsAsync(string username)
        {
            return _context.Users.AnyAsync(x => x.Username == username);
        }

        public Task<bool> IsAdminAsync(int userId)
        {
            return _context.Users
                .Include(u => u.Roles)
                .AnyAsync(u => u.Id == userId && u.Roles.Any(r => r.Name == "Admin"));
        }

        public async Task<User> CreateUserAsync(string username, string passwordHash, bool isActive, DateTime utcNow)
        {
            var user = new User
            {
                Username = username,
                PasswordHash = passwordHash,
                IsActive = isActive,
                CreatedAt = utcNow,
                UpdatedAt = utcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task UpdatePasswordHashAsync(int userId, string passwordHash, DateTime utcNow)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user is null)
                return;

            user.PasswordHash = passwordHash;
            user.UpdatedAt = utcNow;
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int userId, DateTime utcNow)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user is null)
                return;

            user.IsActive = false;
            user.UpdatedAt = utcNow;
            await _context.SaveChangesAsync();
        }

        public async Task<User> UpdateIsActiveAsync(int userId, bool isActive, DateTime utcNow)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user is null)
                throw new InvalidOperationException("User not found.");

            user.IsActive = isActive;
            user.UpdatedAt = utcNow;
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
