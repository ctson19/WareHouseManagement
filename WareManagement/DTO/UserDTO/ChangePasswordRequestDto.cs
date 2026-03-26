namespace WareManagement.DTO.UserDTO;

public class ChangePasswordRequestDto
{
    public string OldPassword { get; set; } = string.Empty;
    public string RenewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

