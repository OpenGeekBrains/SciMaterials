namespace SciMaterials.Contracts.Identity.Clients.Clients.Responses.DTO
{
    public class AuthUsers
    {
        public string Id { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public List<AuthRoles> UserRoles { get; set; } = null!;
    }
}