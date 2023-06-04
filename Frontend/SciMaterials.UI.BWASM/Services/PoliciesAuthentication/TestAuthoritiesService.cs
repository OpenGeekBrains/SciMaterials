using SciMaterials.UI.BWASM.Models;

namespace SciMaterials.UI.BWASM.Services.PoliciesAuthentication;

public class TestAuthoritiesService : IAuthoritiesService
{
    private readonly AuthenticationCache _authenticationCache;
    private readonly IAuthenticationService _authenticationService;

    public TestAuthoritiesService(AuthenticationCache authenticationCache, IAuthenticationService authenticationService)
    {
        _authenticationCache = authenticationCache;
        _authenticationService = authenticationService;
    }

    public List<AuthorityGroup> AuthoritiesGroupsList()
    {
        return _authenticationCache.AuthorityGroupsList();
    }

    public List<Authority> AuthoritiesList()
    {
        return _authenticationCache.AuthoritiesList();
    }

    public async Task Delete(AuthorityGroup authorityGroup)
    {
        _authenticationCache.DeleteAuthorityGroup(authorityGroup.Id, authorityGroup.Name);
        await _authenticationService.RefreshCurrentUser();
    }

    public async Task Delete(Authority authority)
    {
        _authenticationCache.DeleteAuthority(authority.Id);
        await _authenticationService.RefreshCurrentUser();
    }

    public async Task AddAuthorityToGroup(AuthorityGroup group, Authority authority)
    {
        _authenticationCache.AddAuthorityToGroup(group.Id, group.Name, authority.Id);

        await _authenticationService.RefreshCurrentUser();
    }

    public void AddAuthority(string authorityName)
    {
        _authenticationCache.AddAuthority(authorityName);
    }

    public async Task RemoveAuthorityFromGroup(AuthorityGroup group, Authority authority)
    {
        _authenticationCache.RemoveAuthorityFromGroup(group.Id, group.Name, authority.Id);
        await _authenticationService.RefreshCurrentUser();
    }

    public bool AuthoritiesExist(string[] authorities)
    {
        return _authenticationCache.AuthoritiesExist(authorities);
    }

    public void AddAuthorityGroup(string authorityName)
    {
        _authenticationCache.AddAuthorityGroup(authorityName);
    }
}