
namespace SciMaterials.Contracts.Identity.Clients.Clients.Responses.Roles;

public class ClientAddRoleToUserResponse : Result.Result
{
    public string NewToken { get; set; } = null!;
    public string Message { get; set; } = null!;
    public int Code { get; set; }
    public bool Succeeded { get; set; }
}