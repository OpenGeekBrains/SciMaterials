using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts.Identity.Clients.Clients.Responses.User;

public class ClientRefreshTokenResponse : IResult
{
    public string? Message { get; set; }
    public IList<string>? Roles { get; set; }
    public string? NickName { get; set; }
    public string? Email { get; set; }
    public string? RefreshToken { get; set; }
    public int Code { get; set; }
    public bool Succeeded { get; set; }
    
    public ICollection<string> Messages { get; set; }
}