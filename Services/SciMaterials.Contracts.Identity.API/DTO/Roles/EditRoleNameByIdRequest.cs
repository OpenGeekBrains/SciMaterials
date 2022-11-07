namespace SciMaterials.Contracts.Identity.API.DTO.Roles;

public class EditRoleNameByIdRequest
{
    public string RoleId { get; set; } = null!;
    public string RoleName { get; set; } = null!;
}
