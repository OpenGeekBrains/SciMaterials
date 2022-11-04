using System.Security.Claims;

using Blazored.LocalStorage;

using Microsoft.AspNetCore.Components.Authorization;

using SciMaterials.Contracts.API.DTO.AuthUsers;
using SciMaterials.Contracts.Identity.Clients.Clients;
using SciMaterials.UI.BWASM.Models;

namespace SciMaterials.UI.BWASM.Services.Identity;

public class IdentityAuthenticationService : IAuthenticationService
{
    private readonly IIdentityClient _client;
    private readonly ILocalStorageService _localStorageService;
    private readonly IdentityAuthenticationStateProvider _authenticationStateProvider;

    public IdentityAuthenticationService(
        IIdentityClient client,
        ILocalStorageService localStorageService,
        AuthenticationStateProvider authenticationStateProvider)
    {
        _client = client;
        _localStorageService = localStorageService;
        _authenticationStateProvider = (IdentityAuthenticationStateProvider)authenticationStateProvider;
    }

    public async Task Logout()
    {
        var response = await _client.LogoutUserAsync(CancellationToken.None);

        if (!response.Succeeded)
        {
            // handle fail?
            return;
        }

        await _localStorageService.RemoveItemAsync("authToken");
        _authenticationStateProvider.NotifyUserLogout();
    }

    public async Task<bool> SignIn(SignInForm formData)
    {
        // get token
        var response = await _client.LoginUserAsync(
            new LoginRequest
            {
                Email = formData.Email,
                Password = formData.Password
            },
            CancellationToken.None);

        if (!response.Succeeded)
        {
            // TODO: handle failure
            return false;
        }

        var token = response.SessionToken;

        // parse token
        if (token.ParseJwt() is not { Count: > 0 } claims) return false;

        // set user signed with token claims
        await _localStorageService.SetItemAsStringAsync("authToken", token);
        ClaimsIdentity identity = new(claims, "Some Auth Policy Type");
        _authenticationStateProvider.NotifyUserSignIn(identity);

        return true;
    }

    public async Task<bool> SignUp(SignUpForm formData)
    {
        var response = await _client.RegisterUserAsync(
            new RegisterRequest
            {
                Email = formData.Email, 
                NickName = formData.Username,
                Password = formData.Password
            },
            CancellationToken.None);
        if (!response.Succeeded)
        {
            // TODO: handle failure
            return false;
        }

        return true;
    }

    public async Task<bool> IsCurrentUser(string userEmail)
    {
        var currentAuthenticationState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var identifier = currentAuthenticationState.User.FindFirstValue(ClaimTypes.Email);
        return userEmail == identifier;
    }

    public async Task RefreshCurrentUser()
    {
        var currentAuthenticationState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        if(!currentAuthenticationState.User.Identity.IsAuthenticated) return;

        var currentToken = await _localStorageService.GetItemAsStringAsync("authToken");
        // TODO: Refresh Token

        // TODO: Update user claims
    }
}