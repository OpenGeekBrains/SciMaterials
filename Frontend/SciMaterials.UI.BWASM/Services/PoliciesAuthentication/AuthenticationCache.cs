using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

using SciMaterials.Contracts.Result;
using SciMaterials.UI.BWASM.Models;

namespace SciMaterials.UI.BWASM.Services.PoliciesAuthentication;

public class AuthenticationCache
{
    private Dictionary<Guid, Authority> Authorities { get; }
    private Dictionary<string, AuthorityGroup> AuthorityGroups { get; } = new();
    private Dictionary<Guid, AuthorityUserInfo> Users { get; } = new();

    private HashSet<string> AuthoritiesNames { get; }
    private HashSet<string> AuthorityGroupsNames { get; }

    public AuthenticationCache()
    {
        Authority[] authorities =
        {
            Authority.Create("CanAccessUsers"),
            Authority.Create("CanChangeUsersAuthority"),
            Authority.Create("CanDeleteUsers"),
            Authority.Create("CanViewProfile")
        };

        AuthorityGroup adminAuthorityGroup = AuthorityGroup.Create("Admin", authorities[..2]);
        AuthorityGroup userAuthorityGroup = AuthorityGroup.Create("User", authorities[3]);

        Authorities = new(authorities.ToDictionary(e => e.Id));
        AuthorityGroups.TryAdd(adminAuthorityGroup.Name, adminAuthorityGroup);
        AuthorityGroups.TryAdd(userAuthorityGroup.Name, userAuthorityGroup);

        AuthoritiesNames = Authorities.Values.Select(x => x.Name).ToHashSet();
        AuthorityGroupsNames = AuthorityGroups.Values.Select(x => x.Name).ToHashSet();

        AuthorityUserInfo admin = AuthorityUserInfo.Create("Admin", "sa@mail.ru", "test12345", adminAuthorityGroup);
        Users.TryAdd(admin.Id, admin);
    }

    public bool TryAdd(string email, string password, string userName)
    {
        if (Users.Values.FirstOrDefault(x => x.Email == email) is not null) return false;

        AuthorityUserInfo user = AuthorityUserInfo.Create(userName, email, password, AuthorityGroups["User"]);

        // no check on idempotency
        Users.TryAdd(user.Id, user);

        return true;
    }

    public bool TryGetIdentity(string email, string password, [NotNullWhen(true)] out ClaimsIdentity? claimsIdentity, out Guid userId)
    {
        userId = Guid.Empty;
        claimsIdentity = null;
        if (Users.Values.FirstOrDefault(x => x.Email == email && x.Password == password) is not { } user) return false;

        claimsIdentity = GenerateIdentity(user);
        userId = user.Id;
        return true;
    }

    public bool TryGetIdentity(Guid userId, [NotNullWhen(true)] out ClaimsIdentity? identity)
    {
        identity = null;
        if (!Users.TryGetValue(userId, out var user)) return false;
        identity = GenerateIdentity(user);
        return true;
    }

    public List<AuthorityGroup> AuthorityGroupsList()
    {
        return AuthorityGroups.Values.Select(x => AuthorityGroup.Create(x)).ToList();
    }

    public List<AuthorityUserInfo> UsersList()
    {
        return Users.Values.Select(x => AuthorityUserInfo.Create(x)).ToList();
    }

    public Result ChangeAuthorityGroup(string userEmail, string authorityGroupName)
    {
        if (AuthorityGroups.Values.FirstOrDefault(x => x.Name == authorityGroupName) is not { } authorityGroup)
            // just random
            return Result.Failure("242");

        if (Users.Values.FirstOrDefault(x => x.Email == userEmail) is not { } user)
            return Result.Failure("243");

        user.Authority = authorityGroup.Name;
        user.AuthorityGroupId = authorityGroup.Id;
        return Result.Success();
    }

    public Result DeleteUser(string userEmail)
    {
        if (Users.Values.FirstOrDefault(x => x.Email == userEmail) is not { Id: var userId }) return Result.Success();
        return !Users.Remove(userId, out _)
            ? Result.Failure("243")
            : Result.Success();
    }

    public List<Authority> AuthoritiesList()
    {
        return Authorities.Values.ToList();
    }

    public void DeleteAuthority(Guid authorityId)
    {
        if (!Authorities.Remove(authorityId, out var existedAuthority)) return;
        AuthoritiesNames.Remove(existedAuthority.Name);
        foreach (var authorityGroup in AuthorityGroups.Values)
            authorityGroup.Authorities.Remove(existedAuthority);
    }

    public void DeleteAuthorityGroup(Guid authorityGroupId, string authorityGroupName)
    {

        if (authorityGroupName is "User" or "Admin" || !AuthorityGroups.Remove(authorityGroupName, out _)) return;
        AuthorityGroupsNames.Remove(authorityGroupName);
        var userGroup = AuthorityGroups["User"];

        foreach (var user in Users.Values.Where(x => x.AuthorityGroupId == authorityGroupId))
        {
            user.AuthorityGroupId = userGroup.Id;
            user.Authority = userGroup.Name;
        }
    }

    public void AddAuthorityToGroup(Guid groupId, string groupName, Guid authorityId)
    {
        if (!AuthorityGroups.TryGetValue(groupName, out var group) || !Authorities.TryGetValue(authorityId, out var authority)) return;
        // check that authority already exist
        if (group.Authorities.Contains(authority)) return;
        group.Authorities.Add(authority);
    }

    public void AddAuthority(string authorityName)
    {
        if (Authorities.Values.FirstOrDefault(x => x.Name == authorityName) is not null) return;

        Authority newOne = Authority.Create(authorityName);
        if (Authorities.TryAdd(newOne.Id, newOne))
            AuthoritiesNames.Add(authorityName);
    }

    public void RemoveAuthorityFromGroup(Guid groupId, string groupName, Guid authorityId)
    {
        if (!AuthorityGroups.TryGetValue(groupName, out var group) || !Authorities.TryGetValue(authorityId, out var authority)) return;

        group.Authorities.Remove(authority);
    }

    private ClaimsIdentity GenerateIdentity(AuthorityUserInfo user)
    {
        List<Claim> claims = new()
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Email, user.Email)
        };

        if (AuthorityGroups.TryGetValue(user.Authority, out var group))
        {
            claims.Add(new(nameof(Authority), user.Authority));
            claims.AddRange(group.Authorities.Select(x => new Claim(nameof(Authority), x.Name)));
        }

        return new(claims, "InMemoryScheme");
    }

    public bool AuthoritiesExist(string[] authorities)
    {
        return authorities.All(x => AuthorityGroupsNames.Contains(x) || AuthoritiesNames.Contains(x));
    }

    public void AddAuthorityGroup(string authorityName)
    {
        if (AuthorityGroups.Values.FirstOrDefault(x => x.Name == authorityName) is not null) return;

        AuthorityGroup newOne = AuthorityGroup.Create(authorityName);
        if (AuthorityGroups.TryAdd(newOne.Name, newOne))
            AuthoritiesNames.Add(authorityName);
    }
}