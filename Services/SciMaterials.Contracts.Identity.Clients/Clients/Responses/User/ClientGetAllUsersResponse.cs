using Microsoft.AspNetCore.Identity;

using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts.Identity.Clients.Clients.Responses.User;

public class ClientGetAllUsersResponse : IResult
{
    public string? Message { get; set; }
    public List<IdentityUser> Users { get; set; }
    public int Code { get; set; }
    public bool Succeeded { get; set; }
    public ICollection<string> Messages { get; set; }
}