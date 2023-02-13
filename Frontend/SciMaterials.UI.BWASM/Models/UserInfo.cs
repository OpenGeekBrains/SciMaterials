namespace SciMaterials.UI.BWASM.Models;

public class UserInfo
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Authority { get; set; } = null!;
}

public class RolesUserInfo : UserInfo
{
    public UserRole[] UserRoles { get; set; }
}

public class AuthorityUserInfo : UserInfo
{
    public string Password { get; set; } = null!;
    public Guid AuthorityGroupId { get; set; }

    public static AuthorityUserInfo Create(string userName, string email, string password, AuthorityGroup authority) =>
        new()
        {
            UserName = userName,
            Email = email,
            Password = password,
            Authority = authority.Name,
            AuthorityGroupId = authority.Id
        };

    public static AuthorityUserInfo Create(AuthorityUserInfo origin) =>
        new()
        {
            Id = origin.Id,
            UserName = origin.UserName,
            Email = origin.Email,
            Password = origin.Password,
            Authority = origin.Authority,
            AuthorityGroupId = origin.AuthorityGroupId
        };
}