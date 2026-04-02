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

    private async Task EnsureAdminAsync(int adminId)
    {
        if (!await _userRepository.IsAdminAsync(adminId))
            throw new ForbiddenException("B?n khùng cù quy?n th?c hi?n thao tùc nùy.");
    }

    public async Task<UserResponseDto> CreateUserAsync(int adminId, CreateUserRequestDto request)
    {
        await EnsureAdminAsync(adminId);

        if (request is null) throw new ValidationException("Yùu c?u khùng h?p l?.");
        if (string.IsNullOrWhiteSpace(request.UserName)) throw new ValidationException("Tùn ??ng nh?p lù b?t bu?c.");
        if (string.IsNullOrWhiteSpace(request.Password)) throw new ValidationException("M?t kh?u lù b?t bu?c.");
        if (request.UserName.Length > 100) throw new ValidationException("Tùn ??ng nh?p quù dùi.");

        var username = request.UserName.Trim();
        if (await _userRepository.UsernameExistsAsync(username))
            throw new ConflictException("Tùn ??ng nh?p ?ù t?n t?i.");

        var utcNow = DateTime.UtcNow;
        var hash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var user = await _userRepository.CreateUserAsync(username, hash, request.IsActive, utcNow);

        return new UserResponseDto
        {
            Id = user.Id,
            UserName = user.Username,
            IsActive = user.IsActive == true
        };
    }

    public async Task ChangePasswordForMeAsync(int userId, ChangePasswordRequestDto request)
    {
        if (request is null) throw new ValidationException("Yùu c?u khùng h?p l?.");
        if (string.IsNullOrWhiteSpace(request.OldPassword)) throw new ValidationException("M?t kh?u c? lù b?t bu?c.");
        if (string.IsNullOrWhiteSpace(request.RenewPassword)) throw new ValidationException("M?t kh?u m?i lù b?t bu?c.");
        if (request.RenewPassword != request.ConfirmPassword)
            throw new ValidationException("M?t kh?u xùc nh?n khùng kh?p.");

        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null) throw new NotFoundException("Khùng tùm th?y ng??i dùng.");
        if (user.IsActive != true) throw new ValidationException("Tùi kho?n ?ù b? khùa.");

        if (!BCrypt.Net.BCrypt.Verify(request.OldPassword, user.PasswordHash))
            throw new UnauthorizedException("M?t kh?u c? khùng ?ùng.");

        var utcNow = DateTime.UtcNow;
        var hash = BCrypt.Net.BCrypt.HashPassword(request.RenewPassword);
        await _userRepository.UpdatePasswordHashAsync(userId, hash, utcNow);
    }

    public async Task ResetPasswordForUserAsync(int adminId, int userId, ResetPasswordRequestDto request)
    {
        await EnsureAdminAsync(adminId);

        if (request is null) throw new ValidationException("Yùu c?u khùng h?p l?.");
        if (string.IsNullOrWhiteSpace(request.RenewPassword)) throw new ValidationException("M?t kh?u m?i lù b?t bu?c.");
        if (request.RenewPassword != request.ConfirmPassword)
            throw new ValidationException("M?t kh?u x·c nh?n khÙng kh?p.");

        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null) throw new NotFoundException("Khùng tùm th?y ng??i dùng.");

        var utcNow = DateTime.UtcNow;
        var hash = BCrypt.Net.BCrypt.HashPassword(request.RenewPassword);
        await _userRepository.UpdatePasswordHashAsync(userId, hash, utcNow);
    }

    public async Task SoftDeleteAsync(int adminId, int userId)
    {
        await EnsureAdminAsync(adminId);

        if (adminId == userId)
            throw new ValidationException("Khùng th? t? xùa tùi kho?n c?a chùnh mùnh.");

        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null) throw new NotFoundException("Khùng tùm th?y ng??i dùng.");

        var utcNow = DateTime.UtcNow;
        await _userRepository.SoftDeleteAsync(userId, utcNow);
    }

    public async Task<UserResponseDto> UpdateUserStatusAsync(int adminId, int userId, bool isActive)
    {
        await EnsureAdminAsync(adminId);

        if (adminId == userId && !isActive)
            throw new ValidationException("Khùng th? t? khùa tùi kho?n c?a chùnh mùnh.");

        var user = await _userRepository.UpdateIsActiveAsync(userId, isActive, DateTime.UtcNow);
        return new UserResponseDto
        {
            Id = user.Id,
            UserName = user.Username,
            IsActive = user.IsActive == true
        };
    }
}
