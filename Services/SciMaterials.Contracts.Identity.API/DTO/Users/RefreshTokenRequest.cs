namespace SciMaterials.Contracts.Identity.API.DTO.Users;

public class RefreshTokenRequest
{
    public string NickName { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public IList<string> Roles { get; set; }
}