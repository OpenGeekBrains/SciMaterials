namespace SciMaterials.Contracts.Identity.API.DTO.Users;

public class RegisterRequest
{
    public string NickName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}