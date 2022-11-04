namespace SciMaterials.Contracts.Identity.API.DTO.Roles;

public class AddRoleToUserRequest
{
    public string? Email { get; set; }
    public string? RoleName { get; set; }
}