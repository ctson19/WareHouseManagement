using BCrypt.Net;
using WareManagement.DTO.UserDTO;
using WareManagement.Repository.Interfaces;
using WareManagement.Service.Exceptions;
using WareManagement.Service.Interfaces;

namespace WareManagement.Service.Implementations;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserResponseDto> CreateUserAsync(int adminId, CreateUserRequestDto request)
    {
        if (!await _userRepository.IsAdminAsync(adminId))
            throw new ForbiddenException("B?n không có quy?n th?c hi?n hành ??ng này");

        if (request is null)
            throw new ValidationException("Yêu c?u không ???c ?? tr?ng");

        if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
            throw new ValidationException("Tên ??ng nh?p và m?t kh?u là b?t bu?c.");

        var exists = await _userRepository.UsernameExistsAsync(request.UserName);
        if (exists)
            throw new ConflictException("Tên ??ng nh?p ?ã t?n t?i.");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var now = DateTime.UtcNow;

        var user = await _userRepository.CreateUserAsync(
            username: request.UserName,
            passwordHash: passwordHash,
            isActive: request.IsActive,
            utcNow: now);

        return new UserResponseDto
        {
            Id = user.Id,
            UserName = user.Username,
            IsActive = user.IsActive ?? false
        };
    }

    public async Task ChangePasswordForMeAsync(int userId, ChangePasswordRequestDto request)
    {
        if (request is null)
            throw new ValidationException("Yêu c?u không ???c ?? tr?ng.");

        if (string.IsNullOrWhiteSpace(request.OldPassword) || string.IsNullOrWhiteSpace(request.RenewPassword) || string.IsNullOrWhiteSpace(request.ConfirmPassword))
            throw new ValidationException("M?t kh?u c? và m?t kh?u m?i là b?t bu?c.");

        if (!string.Equals(request.RenewPassword, request.ConfirmPassword, StringComparison.Ordinal))
            throw new ValidationException("Xác nh?n m?t kh?u không kh?p v?i m?t kh?u m?i.");

        if (request.RenewPassword.Length < 6)
            throw new ValidationException("M?t kh?u m?i ph?i có ít nh?t 6 ký t?.");

        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
            throw new NotFoundException("Không tìm th?y ng??i dùng.");

        if (user.IsActive != true)
            throw new ValidationException("Ng??i dùng ?ang b? vô hi?u hóa.");

        var ok = BCrypt.Net.BCrypt.Verify(request.OldPassword, user.PasswordHash);
        if (!ok)
            throw new UnauthorizedException("M?t kh?u c? không ?úng.");

        var newHash = BCrypt.Net.BCrypt.HashPassword(request.RenewPassword);
        await _userRepository.UpdatePasswordHashAsync(userId, newHash, DateTime.UtcNow);
    }

    public async Task ResetPasswordForUserAsync(int adminId, int userId, ResetPasswordRequestDto request)
    {
        if (!await _userRepository.IsAdminAsync(adminId))
            throw new ForbiddenException("B?n không có quy?n th?c hi?n hành ??ng này.");

        if (request is null)
            throw new ValidationException("Yêu c?u không ???c ?? tr?ng.");

        if (string.IsNullOrWhiteSpace(request.RenewPassword) || string.IsNullOrWhiteSpace(request.ConfirmPassword))
            throw new ValidationException("M?t kh?u m?i và xác nh?n m?t kh?u là b?t bu?c.");

        if (!string.Equals(request.RenewPassword, request.ConfirmPassword, StringComparison.Ordinal))
            throw new ValidationException("Xác nh?n m?t kh?u không kh?p v?i m?t kh?u m?i.");

        if (request.RenewPassword.Length < 6)
            throw new ValidationException("M?t kh?u m?i ph?i có ít nh?t 6 ký t?.");

        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
            throw new NotFoundException("Không tìm th?y ng??i dùng.");

        if (user.IsActive != true)
            throw new ValidationException("Ng??i dùng ?ang b? vô hi?u hóa.");

        var newHash = BCrypt.Net.BCrypt.HashPassword(request.RenewPassword);
        await _userRepository.UpdatePasswordHashAsync(userId, newHash, DateTime.UtcNow);
    }

    public async Task SoftDeleteAsync(int adminId, int userId)
    {
        if (!await _userRepository.IsAdminAsync(adminId))
            throw new ForbiddenException("B?n không có quy?n th?c hi?n hành ??ng này.");

        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
            throw new NotFoundException("Không tìm th?y ng??i dùng.");

        if (user.IsActive == false)
            throw new ValidationException("Ng??i dùng ?ã b? vô hi?u hóa tr??c ?ó.");

        await _userRepository.SoftDeleteAsync(userId, DateTime.UtcNow);
    }

    public async Task<UserResponseDto> UpdateUserStatusAsync(int adminId, int userId, bool isActive)
    {
        if (!await _userRepository.IsAdminAsync(adminId))
            throw new ForbiddenException("B?n không có quy?n th?c hi?n hành ??ng này.");

        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
            throw new NotFoundException("Không tìm th?y ng??i dùng.");

        var updatedUser = await _userRepository.UpdateIsActiveAsync(userId, isActive, DateTime.UtcNow);
        return new UserResponseDto
        {
            Id = updatedUser.Id,
            UserName = updatedUser.Username,
            IsActive = updatedUser.IsActive ?? false
        };
    }
}
