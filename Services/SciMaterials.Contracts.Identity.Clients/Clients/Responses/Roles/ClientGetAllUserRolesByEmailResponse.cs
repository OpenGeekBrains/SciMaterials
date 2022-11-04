using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts.Identity.Clients.Clients.Responses.Roles;

public class ClientGetAllUserRolesByEmailResponse : IResult
{
    public string? Message { get; set; }
    public IList<string> Roles { get; set; }
    public int Code { get; set; }
    public bool Succeeded { get; set; }
    public ICollection<string> Messages { get; set; }
}