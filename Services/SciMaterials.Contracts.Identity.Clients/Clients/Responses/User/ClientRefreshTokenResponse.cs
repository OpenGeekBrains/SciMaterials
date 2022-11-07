namespace SciMaterials.Contracts.Identity.Clients.Clients.Responses.User;

public class ClientRefreshTokenResponse : Result.Result
{
    public string Message { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public int Code { get; set; }
    public bool Succeeded { get; set; }
}