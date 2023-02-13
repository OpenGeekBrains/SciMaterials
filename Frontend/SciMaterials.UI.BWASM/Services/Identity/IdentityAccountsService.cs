using SciMaterials.Contracts.Identity.API;
using SciMaterials.UI.BWASM.Models;

namespace SciMaterials.UI.BWASM.Services.Identity;

public class IdentityAccountsService : IAccountsService
{
    private readonly IIdentityApi _IdentityApi;
    private readonly IAuthenticationService _AuthenticationService;

    public IdentityAccountsService(
        IIdentityApi IdentityApi,
        IAuthenticationService AuthenticationService)
    {
        _IdentityApi = IdentityApi;
        _AuthenticationService = AuthenticationService;
    }

    public async Task<IReadOnlyList<UserInfo>> UsersList()
    {
        var response = await _IdentityApi.GetAllUsersAsync(CancellationToken.None);
        if (response.IsFaulted)
        {
            // TODO: handle failure
            return Array.Empty<RolesUserInfo>();
        }

        return response.Data.Select(x => new RolesUserInfo()
        {
            Id = Guid.Parse(x.Id),
            UserName = x.UserName,
            Email = x.Email,
            Authority = string.Join(", ", x.UserRoles.Select(c => c.RoleName)),
            UserRoles = x.UserRoles.Select(c => new UserRole() { Id = c.Id, Name = c.RoleName }).ToArray()
        }).ToList();
    }

    public async Task ChangeAuthority(string userEmail, string authorityName)
    {
        await _IdentityApi.DeleteUserRoleByEmailAsync(userEmail, authorityName);
        await _IdentityApi.AddRoleToUserAsync(new() { Email = userEmail, RoleName = authorityName }, CancellationToken.None);

        if (await _AuthenticationService.IsCurrentUser(userEmail))
            await _AuthenticationService.RefreshCurrentUser();
    }

    public async Task Delete(string userEmail)
    {
        var response = await _IdentityApi.DeleteUserByEmailAsync(userEmail);
        if (response.IsFaulted)
        {
            // TODO: handle failure
            return;
        }

        if (await _AuthenticationService.IsCurrentUser(userEmail))
            await _AuthenticationService.RefreshCurrentUser();
    }
}