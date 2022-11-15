using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

using Blazored.LocalStorage;

using Microsoft.AspNetCore.Components.Authorization;

using SciMaterials.Contracts.Enums;
using SciMaterials.Contracts.Result;

namespace SciMaterials.UI.BWASM.Extensions;

// TODO: add memory cache and polly retry
public class JwtAuthenticationHandler : DelegatingHandler
{
    private readonly AuthenticationStateProvider _stateProvider;
    private readonly ILocalStorageService _localStorageService;

    public JwtAuthenticationHandler(AuthenticationStateProvider stateProvider, ILocalStorageService localStorageService)
    {
        _stateProvider = stateProvider;
        _localStorageService = localStorageService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        HttpResponseMessage response;
        var currentState = await _stateProvider.GetAuthenticationStateAsync();
        if (currentState.User.Identity?.IsAuthenticated is not true)
        {
            response = await TryAgain(request, cancellationToken);
            return response;
        }

        var token = await _localStorageService.GetItemAsStringAsync("authToken");

        if (request.Headers.Authorization?.Parameter != token)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        response = await TryAgain(request, cancellationToken);
        return response;
    }

    private async Task<HttpResponseMessage> TryAgain(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            var failure = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(Result.Failure(
                    FailureCodes.Authentication.NoAccessRightsToResource,
                    "User has no rights to access resource")))
            };
            return failure;
        }

        return response;
    }
}