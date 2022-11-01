﻿using SciMaterials.Contracts.Identity.Clients.Clients;
using SciMaterials.UI.BWASM.Models;

namespace SciMaterials.UI.BWASM.Services.Identity;

public class IdentityAccountsService : IAccountsService
{
    private readonly IIdentityClient _usersClient;
    private readonly IRolesClient _rolesClient;
    private readonly IAuthenticationService _authenticationService;

    public IdentityAccountsService(
        IIdentityClient usersClient,
        IRolesClient rolesClient,
        IAuthenticationService authenticationService)
    {
        _usersClient = usersClient;
        _rolesClient = rolesClient;
        _authenticationService = authenticationService;
    }

    public async Task<IReadOnlyList<UserInfo>> UsersList()
    {
        var response = await _usersClient.GetAllUsersAsync(CancellationToken.None);
        if (!response.Succeeded)
        {
            // TODO: handle failure
            return Array.Empty<RolesUserInfo>();
        }
        
        return response.Users.Select(x => new RolesUserInfo()
        {
            Id = Guid.Parse(x.Id),
            UserName = x.UserName,
            Email = x.Email,
            Authority = string.Join(", ", x.UserRoles.Select(c=>c.RoleName)),
            UserRoles = x.UserRoles.Select(c=>new UserRole(){Id = c.Id, Name = c.RoleName}).ToArray()
        }).ToList();
    }

    public async Task ChangeAuthority(string userEmail, string authorityName)
    {
        await _rolesClient.DeleteUserRoleByEmailAsync(userEmail, authorityName);
        await _rolesClient.AddRoleToUserAsync(new() { Email = userEmail, RoleName = authorityName }, CancellationToken.None);

        if(await _authenticationService.IsCurrentUser(userEmail))
            await _authenticationService.RefreshCurrentUser();
    }

    public async Task Delete(string userEmail)
    {
        var response = await _usersClient.DeleteUserByEmailAsync(userEmail);
        if (!response.Succeeded)
        {
            // TODO: handle failure
            return;
        }

        if (await _authenticationService.IsCurrentUser(userEmail))
            await _authenticationService.RefreshCurrentUser();
    }
}