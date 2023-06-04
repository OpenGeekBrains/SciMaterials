namespace SciMaterials.Contracts.Identity.API.Responses.DTO
{
    public class AuthUser
    {
        public string Id { get; init; } = null!;
        public string UserName { get; init; } = null!;
        public string Email { get; init; } = null!;
        public List<AuthRole> UserRoles { get; init; } = null!;
    }
}