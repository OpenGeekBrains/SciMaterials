namespace SciMaterials.Contracts.Identity.Clients.Clients.Responses.User;

public class ClientCreateUserResponse : Result.Result
{
    public string ConfirmEmail { get; set; } = null!;
    public string Message { get; set; } = null!;
    public int Code { get; set; }
    public bool Succeeded { get; set; }
}