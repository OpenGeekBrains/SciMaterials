using SciMaterials.Contracts.Identity.API;
using SciMaterials.UI.BWASM.Models;

namespace SciMaterials.UI.BWASM.Services.Identity;

public class IdentityRolesService : IRolesService
{
    private readonly IRolesApi _RolesApi;
    private readonly IAuthenticationService _AuthenticationService;

    public IdentityRolesService(
        IRolesApi RolesApi,
        IAuthenticationService AuthenticationService)
    {
        _RolesApi = RolesApi;
        _AuthenticationService = AuthenticationService;
    }

    public async Task<IReadOnlyList<UserRole>> RolesList()
    {
        var response = await _RolesApi.GetAllRolesAsync(CancellationToken.None);
        if (response.IsFaulted) return new List<UserRole>();

        return response.Data.Select(x => new UserRole { Id = x.Id, Name = x.RoleName }).ToList();
    }

    public async Task<bool> AddRole(string roleName)
    {
        var response = await _RolesApi.CreateRoleAsync(new() { RoleName = roleName });
        if (response.Succeeded) return true;

        // TODO: handle failure
        return false;
    }

    public async Task<bool> DeleteRole(string roleId)
    {
        var response = await _RolesApi.DeleteRoleByIdAsync(roleId);
        if (response.IsFaulted)
        {
            // TODO: handle failure
            return false;
        }

        // Validate that current user in role
        // Refresh if it is
        await _AuthenticationService.RefreshCurrentUser();

        return true;
    }
}