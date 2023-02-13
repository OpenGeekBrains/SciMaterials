using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

using System.Security.Claims;

namespace SciMaterials.UI.BWASM.Services.Identity;

public class IdentityAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorageService;
    private static readonly AuthenticationState Anonymous;

    static IdentityAuthenticationStateProvider() => Anonymous = new(new(new ClaimsIdentity()));

    public IdentityAuthenticationStateProvider(ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _localStorageService.GetItemAsStringAsync("authToken");
        if (string.IsNullOrWhiteSpace(token) || token.ParseJwt() is not {Count:>0} claims)
        {
            await _localStorageService.RemoveItemAsync("authToken");
            return Anonymous;
        }

        return new(new(new ClaimsIdentity(claims, "Identity")));
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