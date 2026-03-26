using WareManagement.DTO.Authen;
using WareManagement.DTO.AuthenDTO;

namespace WareManagement.Service.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto request, string? ipAddress);
        Task LogoutAsync(string? username, string? ipAddress);
    }
}
