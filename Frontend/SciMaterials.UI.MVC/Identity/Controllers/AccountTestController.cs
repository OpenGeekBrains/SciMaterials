using Microsoft.AspNetCore.Mvc;
using SciMaterials.Contracts.Identity.API;
using SciMaterials.Contracts.Identity.API.Requests.Roles;
using SciMaterials.Contracts.Identity.API.Requests.Users;

namespace SciMaterials.UI.MVC.Identity.Controllers;

/// <summary>Тестовый контроллер с IdentityApi</summary>
[ApiController]
[Route("account_test/")]
public class AccountTestController : Controller
{
    private readonly IIdentityApi _IdentityApi;
    public AccountTestController(IIdentityApi IdentityApi)
    {
        _IdentityApi = IdentityApi;
    }
    
    [HttpPost(AuthApiRoute.Register)]
    public async Task<IActionResult> RegisterAsync(
        [FromBody] RegisterRequest? RegisterRequest, 
        CancellationToken Cancel = default)
    {
        var response = await _IdentityApi.RegisterUserAsync(RegisterRequest, Cancel);
        return Ok(response.Data?.ConfirmEmail);
    }
    
    [HttpPost(AuthApiRoute.Login)]
    public async Task<IActionResult> LoginAsync(
        [FromBody] LoginRequest? LoginRequest, 
        CancellationToken Cancel = default)
    {
        var response = await _IdentityApi.LoginUserAsync(LoginRequest, Cancel);
        return Ok(response.Data?.SessionToken);
    }
    
    [HttpPost(AuthApiRoute.Logout)]
    public async Task<IActionResult> LogoutAsync(CancellationToken Cancel = default)
    {
        var response = await _IdentityApi.LogoutUserAsync(Cancel);
        return Ok(response.Message);
    }
    
    [HttpPost(AuthApiRoute.ChangePassword)]
    public async Task<IActionResult> ChangePasswordAsync(
        [FromBody] ChangePasswordRequest? PasswordRequest, 
        CancellationToken Cancel = default)
    {
        var response = await _IdentityApi.ChangePasswordAsync(PasswordRequest, Cancel);
        return Ok(response.Message);
    }
    
    [HttpPost(AuthApiRoute.CreateRole)]
    public async Task<IActionResult> CreateRoleAsync(
        CreateRoleRequest CreateRoleRequest, 
        CancellationToken Cancel = default)
    {
        var response = await _IdentityApi.CreateRoleAsync(CreateRoleRequest, Cancel);
        return Ok(response.Message);
    }
    
    [HttpGet(AuthApiRoute.GetAllRoles)]
    public async Task<IActionResult> GetAllRolesAsync(CancellationToken Cancel = default)
    {
        var response = await _IdentityApi.GetAllRolesAsync(Cancel);
        return Ok(response);
    }
    
    [HttpGet($"{AuthApiRoute.GetRoleById}"+"{RoleId}")]
    public async Task<IActionResult> GetRoleByIdAsync(string RoleId, CancellationToken Cancel = default)
    {
        var response = await _IdentityApi.GetRoleByIdAsync(RoleId, Cancel);
        return Ok(response);
    }
    
    [HttpPut(AuthApiRoute.EditRoleNameById)]
    public async Task<IActionResult> EditRoleNameByIdAsync(
        [FromBody] EditRoleNameByIdRequest EditRoleNameByIdRequest, 
        CancellationToken Cancel = default)
    {
        var response = await _IdentityApi.EditRoleNameByIdAsync(EditRoleNameByIdRequest, Cancel);
        return Ok(response.Message);
    }
    
    [HttpDelete($"{AuthApiRoute.DeleteRoleById}"+"{RoleId}")]
    public async Task<IActionResult> DeleteRoleByIdAsync(string RoleId, CancellationToken Cancel = default)
    {
        var response = await _IdentityApi.DeleteRoleByIdAsync(RoleId, Cancel);
        return Ok(response.Message);
    }
    
    [HttpPost(AuthApiRoute.AddRoleToUser)]
    public async Task<IActionResult> AddRoleToUserAsync(
        [FromBody] AddRoleToUserRequest? AddRoleRequest, 
        CancellationToken Cancel = default)
    {
        var response = await _IdentityApi.AddRoleToUserAsync(AddRoleRequest, Cancel);
        return Ok(response);
    }
    
    [HttpDelete($"{AuthApiRoute.DeleteUserRoleByEmail}"+"{Email}/{RoleName}")]
    public async Task<IActionResult> DeleteUserRoleByEmailAsync(
        string Email,
        string RoleName, 
        CancellationToken Cancel = default)
    {
        var response = await _IdentityApi.DeleteUserRoleByEmailAsync(Email, RoleName, Cancel);
        return Ok(response.Message);
    }
    
    [HttpGet($"{AuthApiRoute.GetAllUserRolesByEmail}"+"{Email}")]
    public async Task<IActionResult> GetAllUserRolesByEmailAsync(string Email, CancellationToken Cancel = default)
    {
        var response = await _IdentityApi.GetUserRolesAsync(Email, Cancel);
        return Ok(response);
    }
    
    [HttpGet($"{AuthApiRoute.GetUserByEmail}"+"{Email}")]
    public async Task<IActionResult> GetUserByEmailAsync(string Email, CancellationToken Cancel = default)
    {
        var response = await _IdentityApi.GetUserByEmailAsync(Email, Cancel);
        return Ok(response);
    }
    
    [HttpGet(AuthApiRoute.GetAllUsers)]
    public async Task<IActionResult> GetAllUsersAsync(CancellationToken Cancel = default)
    {
        var response = await _IdentityApi.GetAllUsersAsync(Cancel);
        return Ok(response);
    }
    
    [HttpPut(AuthApiRoute.EditUserByEmail)]
    public async Task<IActionResult> EditUserNameByEmailAsync(
        [FromBody] EditUserNameByEmailRequest? EditUserRequest, 
        CancellationToken Cancel = default)
    {
        var response = await _IdentityApi.EditUserNameByEmailAsync(EditUserRequest, Cancel);
        return Ok(response);
    }
    
    [HttpDelete($"{AuthApiRoute.DeleteUserByEmail}"+"{Email}")]
    public async Task<IActionResult> DeleteUserByEmailAsync(string Email, CancellationToken Cancel = default)
    {
        var response = await _IdentityApi.DeleteUserByEmailAsync(Email, Cancel);
        return Ok(response.Message);
    }

    [HttpGet(AuthApiRoute.RefreshToken)]
    public async Task<IActionResult> GetRefreshTokenAsync(CancellationToken Cancel = default)
    {
        var response = await _IdentityApi.GetRefreshTokenAsync(Cancel);
        return Ok(response.Data?.RefreshToken);
    }
}