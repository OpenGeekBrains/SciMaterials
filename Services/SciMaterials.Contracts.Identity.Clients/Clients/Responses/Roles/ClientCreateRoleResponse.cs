using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts.Identity.Clients.Clients.Responses.Roles;

public class ClientCreateRoleResponse : IResult
{
    public string? Message { get; set; }
    public int Code { get; set; }
    public bool Succeeded { get; set; }
    public ICollection<string> Messages { get; set; }
}