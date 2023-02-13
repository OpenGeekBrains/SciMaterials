namespace SciMaterials.Contracts.Identity.API.Requests.Roles;

public class AddRoleToUserRequest
{
    public string Email { get; init; } = null!;
    public string RoleName { get; init; } = null!;
}