using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

using static SciMaterials.UI.BWASM.Services.PoliciesAuthentication.AuthorityRules;

namespace SciMaterials.UI.BWASM.Services.PoliciesAuthentication;

public class AuthorityRequirement : IAuthorizationRequirement
{
    public AuthorityRequirement(string[] authorities)
    {
        Authorities = authorities;
    }

    public string[] Authorities { get; }
}

public class AuthorityHandler : AuthorizationHandler<AuthorityRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorityRequirement requirement)
    {
        // receive requirement and after check it say context.Succeed or Fail
        // for now any of authority may pass
        if (requirement.Authorities.Any(x => context.User.HasClaim("Authority", x)))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        context.Fail();
        return Task.CompletedTask;
    }
}

public static class AuthorityRules
{
    public const string Separator = ", ";

    public static string[] GetAuthoritiesFromPolicy(string policyName) =>
        policyName
            .Split(Separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}

/// <summary>
/// Pointless, but can be used instead of [Authorize(Policy="")]
/// with custom logic for building policyName that will be handled in PolicyProvider
/// </summary>
public class AuthorityAttribute : AuthorizeAttribute
{
    public AuthorityAttribute(params string[] authorities)
    {
        Policy = string.Join(Separator, authorities);
    }
}

public class AuthorityPolicyProvider : DefaultAuthorizationPolicyProvider
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public AuthorityPolicyProvider(IOptions<AuthorizationOptions> options, IServiceScopeFactory serviceScopeFactory) : base(options)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        var authorities = GetAuthoritiesFromPolicy(policyName);

        using var scope = _serviceScopeFactory.CreateScope();
        var authoritiesService = scope.ServiceProvider.GetRequiredService<IAuthoritiesService>();
        // if one of authorities not registered, will handle section as default
        if (!authoritiesService.AuthoritiesExist(authorities))
            return await base.GetPolicyAsync(policyName);

        AuthorityRequirement requirement = new(authorities);

        AuthorizationPolicy authorizationPolicy = new AuthorizationPolicyBuilder()
            .AddRequirements(requirement)
            .Build();

        return authorizationPolicy;
    }
}