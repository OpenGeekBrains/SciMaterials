using System.Text.Json;

using Microsoft.AspNetCore.Identity;

using SciMaterials.Contracts.Identity.Clients.Clients;
using SciMaterials.UI.BWASM.Models;

namespace SciMaterials.UI.BWASM.Services.Identity;

public class IdentityRolesService : IRolesService
{
    private readonly IRolesClient _rolesClient;
    private readonly IAuthenticationService _authenticationService;

    public IdentityRolesService(
        IRolesClient rolesClient,
        IAuthenticationService authenticationService)
    {
        _rolesClient = rolesClient;
        _authenticationService = authenticationService;
    }

    public async Task<List<IdentityRole>> RolesList()
    {
        var response = await _rolesClient.GetAllRolesAsync(CancellationToken.None);
        if (!response.Succeeded) return new();

        return response.Roles;
    }

    public async Task<bool> AddRole(string roleName)
    {
        var response = await _rolesClient.CreateRoleAsync(new() { RoleName = roleName });
        if (response.Succeeded) return true;

        // TODO: handle failure
        return false;

    }

    public async Task<bool> DeleteRole(string roleId)
    {
        var response = await _rolesClient.DeleteRoleByIdAsync(roleId);
        if (!response.Succeeded)
        {
            // TODO: handle failure
            return false;
        }

        // Validate that current user in role
        // Refresh if it is

        return true;
    }
}