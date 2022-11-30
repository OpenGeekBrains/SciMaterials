namespace SciMaterials.Contracts.Identity.API.Requests.Users;

public class RegisterRequest
{
    public string NickName { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
}