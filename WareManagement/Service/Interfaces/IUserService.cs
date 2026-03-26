using WareManagement.DTO.UserDTO;

namespace WareManagement.Service.Interfaces;

public interface IUserService
{
    Task<UserResponseDto> CreateUserAsync(int adminId, CreateUserRequestDto request);
    Task ChangePasswordForMeAsync(int userId, ChangePasswordRequestDto request);
    Task ResetPasswordForUserAsync(int adminId, int userId, ResetPasswordRequestDto request);
    Task SoftDeleteAsync(int adminId, int userId);
    Task<UserResponseDto> UpdateUserStatusAsync(int adminId, int userId, bool isActive);
}
