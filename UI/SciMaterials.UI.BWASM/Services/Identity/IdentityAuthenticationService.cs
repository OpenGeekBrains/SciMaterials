using System.Security.Claims;

using Blazored.LocalStorage;

using Microsoft.AspNetCore.Components.Authorization;

using SciMaterials.Contracts.Identity.API;
using SciMaterials.Contracts.Identity.API.Requests.Users;
using SciMaterials.UI.BWASM.Models;

namespace SciMaterials.UI.BWASM.Services.Identity;

public class IdentityAuthenticationService : IAuthenticationService
{
    private readonly IUsersApi _Api;
    private readonly ILocalStorageService _LocalStorageService;
    private readonly IdentityAuthenticationStateProvider _AuthenticationStateProvider;

    public IdentityAuthenticationService(
        IUsersApi Api,
        ILocalStorageService LocalStorageService,
        AuthenticationStateProvider AuthenticationStateProvider)
    {
        _Api = Api;
        _LocalStorageService = LocalStorageService;
        _AuthenticationStateProvider = (IdentityAuthenticationStateProvider)AuthenticationStateProvider;
    }

    public async Task Logout()
    {
        var response = await _Api.LogoutUserAsync(CancellationToken.None);

        if (!response.Succeeded)
        {
            // handle fail?
            return;
        }

        await _LocalStorageService.RemoveItemAsync("authToken");
        _AuthenticationStateProvider.NotifyUserLogout();
    }

    public async Task<bool> SignIn(SignInForm formData)
    {
        // get token
        var response = await _Api.LoginUserAsync(
            new LoginRequest
            {
                Email = formData.Email,
                Password = formData.Password
            },
            CancellationToken.None);

        if (response.IsFaulted)
        {
            // TODO: handle failure
            return false;
        }

        var token = response.Data.SessionToken;

        // parse token
        if (token.ParseJwt() is not { Count: > 0 } claims) return false;

        // set user signed with token claims
        await _LocalStorageService.SetItemAsStringAsync("authToken", token);
        ClaimsIdentity identity = new(claims, "Jwt");
        _AuthenticationStateProvider.NotifyUserSignIn(identity);

        return true;
    }

    public async Task<bool> SignUp(SignUpForm formData)
    {
        var response = await _Api.RegisterUserAsync(
            new RegisterRequest
            {
                Email = formData.Email, 
                NickName = formData.Username,
                Password = formData.Password
            },
            CancellationToken.None);
        if (response.IsFaulted)
        {
            // TODO: handle failure
            return false;
        }

        return true;
    }

    public async Task<bool> IsCurrentUser(string userEmail)
    {
        var currentAuthenticationState = await _AuthenticationStateProvider.GetAuthenticationStateAsync();
        var identifier = currentAuthenticationState.User.FindFirst(ClaimTypes.Email)?.Value;
        return userEmail == identifier;
    }

    public async Task RefreshCurrentUser()
    {
        var currentAuthenticationState = await _AuthenticationStateProvider.GetAuthenticationStateAsync();
        if(!currentAuthenticationState.User.Identity.IsAuthenticated) return;
        
        var response = await _Api.GetRefreshTokenAsync();
        if (response.IsFaulted || response.Data.RefreshToken.ParseJwt() is not {Count:>0} claims)
        {
            _AuthenticationStateProvider.NotifyUserLogout();
            return;
        }

        await _LocalStorageService.SetItemAsStringAsync("authToken", response.Data.RefreshToken);
        _AuthenticationStateProvider.NotifyUserSignIn(new ClaimsIdentity(claims, "Jwt"));
    }
}