
namespace SciMaterials.Contracts.Identity.Clients.Clients.Responses.Roles;

public class ClientDeleteRoleByIdResponse : Result.Result
{
    public string Message { get; set; } = null!;
    public int Code { get; set; }
    public bool Succeeded { get; set; }
}