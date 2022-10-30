using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts.API.DTO.Clients;

public class IdentityClientResponse : IResult
{
    public string? ConfirmEmail { get; set; }
    public string? Content { get; set; }
    public int Code { get; set; }
    public bool Succeeded { get; set; }
    public ICollection<string> Messages { get; set; }
}