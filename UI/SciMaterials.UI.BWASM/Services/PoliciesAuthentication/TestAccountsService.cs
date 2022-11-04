using Microsoft.AspNetCore.Components.Authorization;

using SciMaterials.UI.BWASM.Models;

namespace SciMaterials.UI.BWASM.Services.PoliciesAuthentication;

public class TestAccountsService : IAccountsService
{
    private readonly AuthenticationCache _authenticationCache;
    private readonly IAuthenticationService _authenticationService;

    public TestAccountsService(AuthenticationCache authenticationCache, IAuthenticationService authenticationService)
    {
        _authenticationCache = authenticationCache;
        _authenticationService = authenticationService;
    }

    public Task<IReadOnlyList<UserInfo>> UsersList()
    {
        // take view models
        var data = _authenticationCache.UsersList();
        return Task.FromResult<IReadOnlyList<UserInfo>>(data);
    }

    public async Task ChangeAuthority(string userEmail, string authorityName)
    {
        var result = _authenticationCache.ChangeAuthorityGroup(userEmail, authorityName);

        if (!result.Succeeded)
            // TODO: handle failure
            return;
        if (await _authenticationService.IsCurrentUser(userEmail))
            await _authenticationService.RefreshCurrentUser();
    }

    public async Task Delete(string userEmail)
    {
        _authenticationCache.DeleteUser(userEmail);

        if (await _authenticationService.IsCurrentUser(userEmail))
            await _authenticationService.RefreshCurrentUser();
    }
}