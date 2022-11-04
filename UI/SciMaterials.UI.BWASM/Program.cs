using Blazored.LocalStorage;

using Fluxor;

using FluentValidation;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using MudBlazor.Services;

using SciMaterials.Contracts.Identity.Clients.Clients;
using SciMaterials.Contracts.WebApi.Clients.Authors;
using SciMaterials.UI.BWASM;
using SciMaterials.UI.BWASM.Extensions;
using SciMaterials.UI.BWASM.Models;
using SciMaterials.UI.BWASM.Models.Validations;
using SciMaterials.UI.BWASM.Services;
using SciMaterials.UI.BWASM.Services.Identity;
using SciMaterials.WebApi.Clients.Authors;
using SciMaterials.WebApi.Clients.Extensions;
using SciMaterials.WebApi.Clients.Identity;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Nuget
builder.Services
    .AddMudServices()
    .AddBlazoredLocalStorage();

// Api
string apiRoot = builder.HostEnvironment.BaseAddress;

builder.Services
    .AddScoped<JwtAuthenticationHandler>()
    .AddApiClients(new Uri(apiRoot))
    .AddApiClient<IAuthorsClient, AuthorsClient>(apiRoot)
    .AddApiClient<IIdentityClient, IdentityClient>(apiRoot, ClientConfiguration)
    .AddApiClient<IUserClient, IdentityClient>(apiRoot, ClientConfiguration)
    .AddApiClient<IRolesClient, IdentityClient>(apiRoot, ClientConfiguration);

// Authentication
builder.Services
    .AddAuthorizationCore()
    .AddScoped<IAuthenticationService, IdentityAuthenticationService>()
    .AddScoped<AuthenticationStateProvider, IdentityAuthenticationStateProvider>()
    .AddScoped<IRolesService, IdentityRolesService>()
    .AddScoped<IAccountsService, IdentityAccountsService>()
    //.AddScoped<IAuthoritiesService, TestAuthoritiesService>()
    //.AddSingleton<AuthenticationCache>()
    //.AddSingleton<IAuthorizationHandler, AuthorityHandler>()
    //.AddSingleton<IAuthorizationPolicyProvider, AuthorityPolicyProvider>()
    ;

// Validators // TODO: register with assembly scan
builder.Services
    .AddScoped<IValidator<SignUpForm>, SignUpFormValidator>()
    .AddScoped<IValidator<SignInForm>, SignInFormValidator>();

// Background
builder.Services
    .AddSingleton<FileUploadScheduleService>();

// State management
builder.Services.AddFluxor(options =>
{
    options.WithLifetime(StoreLifetime.Singleton);
    options.ScanAssemblies(typeof(Program).Assembly);
    options.UseReduxDevTools(reduxOptions => reduxOptions.Name = "SciMaterials");
});

await builder.Build().RunAsync();

static void ClientConfiguration(IHttpClientBuilder c) => c.AddHttpMessageHandler<JwtAuthenticationHandler>();