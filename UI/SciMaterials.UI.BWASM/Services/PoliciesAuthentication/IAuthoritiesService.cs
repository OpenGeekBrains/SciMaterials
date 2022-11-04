using SciMaterials.UI.BWASM.Models;

namespace SciMaterials.UI.BWASM.Services.PoliciesAuthentication;

public interface IAuthoritiesService
{
    List<AuthorityGroup> AuthoritiesGroupsList();
    List<Authority> AuthoritiesList();
    Task Delete(AuthorityGroup authorityGroup);
    Task Delete(Authority authority);
    Task AddAuthorityToGroup(AuthorityGroup group, Authority authority);
    void AddAuthority(string authorityName);
    Task RemoveAuthorityFromGroup(AuthorityGroup group, Authority authority);
    bool AuthoritiesExist(string[] authorities);
    void AddAuthorityGroup(string authorityName);
}