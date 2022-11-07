
using SciMaterials.Contracts.Identity.Clients.Clients.Responses.DTO;

namespace SciMaterials.Contracts.Identity.Clients.Clients.Responses.Roles;

public class ClientGetAllRolesResponse : Result.Result
{
    public string Message { get; set; } = null!;
    public List<AuthRoles> Roles { get; set; } = null!;
    public int Code { get; set; }
    public bool Succeeded { get; set; }
}