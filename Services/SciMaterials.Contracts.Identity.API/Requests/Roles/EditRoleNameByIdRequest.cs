namespace SciMaterials.Contracts.Identity.API.Requests.Roles;

public class EditRoleNameByIdRequest
{
    public string RoleId { get; init; } = null!;
    public string RoleName { get; init; } = null!;
}
