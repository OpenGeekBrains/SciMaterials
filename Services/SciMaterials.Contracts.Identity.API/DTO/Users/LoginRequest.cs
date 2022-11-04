namespace SciMaterials.Contracts.API.DTO.AuthUsers;

public class LoginRequest
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}