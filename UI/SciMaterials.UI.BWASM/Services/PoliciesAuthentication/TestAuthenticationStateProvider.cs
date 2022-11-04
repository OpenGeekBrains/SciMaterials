using System.Security.Claims;

using Blazored.LocalStorage;

using Microsoft.AspNetCore.Components.Authorization;

namespace SciMaterials.UI.BWASM.Services.PoliciesAuthentication;

public class TestAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorageService;
    private readonly AuthenticationCache _authenticationCache;
    private static readonly AuthenticationState Anonymous;

    static TestAuthenticationStateProvider() => Anonymous = new(new(new ClaimsIdentity()));

    public TestAuthenticationStateProvider(ILocalStorageService localStorageService, AuthenticationCache authenticationCache)
    {
        _localStorageService = localStorageService;
        _authenticationCache = authenticationCache;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _localStorageService.GetItemAsStringAsync("authToken");
        if (string.IsNullOrWhiteSpace(token)
            || !Guid.TryParse(token, out var userId)
            || !_authenticationCache.TryGetIdentity(userId, out var identity))
        {
            await _localStorageService.RemoveItemAsync("authToken");
            return Anonymous;
        }

        return new(new(identity));
    }

    public void NotifyUserSignIn(ClaimsIdentity userClaims)
    {
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new(userClaims))));
    }

    public void NotifyUserLogout()
    {
        NotifyAuthenticationStateChanged(Task.FromResult(Anonymous));
    }
}