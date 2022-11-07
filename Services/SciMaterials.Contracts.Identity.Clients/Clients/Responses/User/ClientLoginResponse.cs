namespace SciMaterials.Contracts.Identity.Clients.Clients.Responses.User
{
    public class ClientLoginResponse : Result.Result
    {
        public string SessionToken { get; set; } = null!;
        public string Message { get; set; } = null!;
        public int Code { get; set; }
        public bool Succeeded { get; set; }
    }
}