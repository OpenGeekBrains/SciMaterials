using System.Security.Claims;

using Blazored.LocalStorage;

using Microsoft.AspNetCore.Components.Authorization;

using SciMaterials.UI.BWASM.Models;

namespace SciMaterials.UI.BWASM.Services.PoliciesAuthentication;

public class TestAuthenticationService : IAuthenticationService
{
    private readonly ILocalStorageService _localStorageService;
    private readonly TestAuthenticationStateProvider _authenticationStateProvider;
    private readonly AuthenticationCache _authenticationCache;

    public TestAuthenticationService(
        ILocalStorageService localStorageService,
        AuthenticationStateProvider authenticationStateProvider,
        AuthenticationCache authenticationCache)
    {
        _localStorageService = localStorageService;
        _authenticationStateProvider = (TestAuthenticationStateProvider)authenticationStateProvider;
        _authenticationCache = authenticationCache;
    }

    public Task<bool> SignUp(SignUpForm formData)
    {
        return Task.FromResult(_authenticationCache.TryAdd(formData.Email, formData.Password, formData.Username));
    }

    public async Task<bool> SignIn(SignInForm formData)
    {
        if (!_authenticationCache.TryGetIdentity(formData.Email!, formData.Password!, out var identity, out var userId))
            return false;

        await _localStorageService.SetItemAsStringAsync("authToken", userId.ToString());

        _authenticationStateProvider.NotifyUserSignIn(identity);

        return true;
    }

    public async Task Logout()
    {
        await _localStorageService.RemoveItemAsync("authToken");
        _authenticationStateProvider.NotifyUserLogout();
    }

    public async Task<bool> IsCurrentUser(string userEmail)
    {
        var currentAuthenticationState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var identifier = currentAuthenticationState.User.FindFirst(ClaimTypes.Email)?.Value;
        return userEmail == identifier;
    }

    public async Task RefreshCurrentUser()
    {
        var currentAuthenticationState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var identifier = currentAuthenticationState.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(identifier, out var userId) && _authenticationCache.TryGetIdentity(userId, out var identity))
        {
            _authenticationStateProvider.NotifyUserSignIn(identity);
            return;
        }
        _authenticationStateProvider.NotifyUserLogout();
    }
}