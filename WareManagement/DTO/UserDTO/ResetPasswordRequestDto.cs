namespace WareManagement.DTO.UserDTO;

public class ResetPasswordRequestDto
{
    public string RenewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

