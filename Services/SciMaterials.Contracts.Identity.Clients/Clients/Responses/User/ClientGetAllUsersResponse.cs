using SciMaterials.Contracts.Identity.Clients.Clients.Responses.DTO;

namespace SciMaterials.Contracts.Identity.Clients.Clients.Responses.User;

public class ClientGetAllUsersResponse : Result.Result
{
    public string Message { get; set; } = null!;
    public List<AuthUsers> Users { get; set; } = null!;
    public int Code { get; set; }
    public bool Succeeded { get; set; }
}