namespace SciMaterials.Contracts.Identity.API.Requests.Users;

public class LoginRequest
{
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
}