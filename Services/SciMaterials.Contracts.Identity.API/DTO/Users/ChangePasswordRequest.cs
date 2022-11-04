namespace SciMaterials.Contracts.API.DTO.AuthUsers;

public class ChangePasswordRequest
{
    public string? CurrentPassword { get; set; }
    public string? NewPassword { get; set; }
}