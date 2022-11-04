using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts.Identity.Clients.Clients.Responses;

public class ClientLoginResponse : IResult
{
    public string? SessionToken { get; set; }
    public string? Message { get; set; }
    public int Code { get; set; }
    public bool Succeeded { get; set; }
    
    public ICollection<string> Messages { get; set; }
}