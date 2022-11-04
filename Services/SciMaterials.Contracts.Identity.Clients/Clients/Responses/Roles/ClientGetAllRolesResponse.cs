using Microsoft.AspNetCore.Identity;
using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts.Identity.Clients.Clients.Responses.Roles;

public class ClientGetAllRolesResponse : IResult
{
    public string? Message { get; set; }
    public List<IdentityRole> Roles { get; set; }
    public int Code { get; set; }
    public bool Succeeded { get; set; }
    public ICollection<string> Messages { get; set; }
}