using Microsoft.AspNetCore.Identity;
using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts.Identity.Clients.Clients.Responses.Roles;

public class ClientGetRoleByIdResponse : IResult
{
    public string? Message { get; set; }
    public IdentityRole Role { get; set; }
    public int Code { get; set; }
    public bool Succeeded { get; set; }
    public ICollection<string> Messages { get; set; }
}