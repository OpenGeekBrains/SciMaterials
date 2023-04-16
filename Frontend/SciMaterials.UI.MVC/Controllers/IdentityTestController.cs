using Microsoft.AspNetCore.Mvc;
using SciMaterials.Contracts.Identity.API;
using SciMaterials.Contracts.Identity.API.Requests.Roles;
using SciMaterials.Contracts.Identity.API.Requests.Users;

namespace SciMaterials.UI.MVC.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IdentityTestController : ControllerBase
{
	private readonly IIdentityApi _IdentityApi;
	public IdentityTestController(IIdentityApi identityApi) => _IdentityApi = identityApi;

	[HttpPost(AuthApiRoute.Register)]
	public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest? registerRequest, CancellationToken cancel = default)
	{
		var response = await _IdentityApi.RegisterUserAsync(registerRequest, cancel);
		return Ok(response.Data?.ConfirmEmail);
	}

	[HttpPost(AuthApiRoute.Login)]
	public async Task<IActionResult> LoginAsync([FromBody] LoginRequest? loginRequest, CancellationToken cancel = default)
	{
		var response = await _IdentityApi.LoginUserAsync(loginRequest, cancel);
		return Ok(response.Data?.SessionToken);
	}

	[HttpPost(AuthApiRoute.Logout)]
	public async Task<IActionResult> LogoutAsync(CancellationToken cancel = default)
	{
		var response = await _IdentityApi.LogoutUserAsync(cancel);
		return Ok(response.Message);
	}

	[HttpPost(AuthApiRoute.ChangePassword)]
	public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordRequest? passwordRequest, CancellationToken cancel = default)
	{
		var response = await _IdentityApi.ChangePasswordAsync(passwordRequest, cancel);
		return Ok(response.Message);
	}

	[HttpPost(AuthApiRoute.CreateRole)]
	public async Task<IActionResult> CreateRoleAsync(CreateRoleRequest createRoleRequest, CancellationToken cancel = default)
	{
		var response = await _IdentityApi.CreateRoleAsync(createRoleRequest, cancel);
		return Ok(response.Message);
	}

	[HttpGet(AuthApiRoute.GetAllRoles)]
	public async Task<IActionResult> GetAllRolesAsync(CancellationToken cancel = default)
	{
		var response = await _IdentityApi.GetAllRolesAsync(cancel);
		return Ok(response);
	}

	[HttpGet($"{AuthApiRoute.GetRoleById}/{{roleId}}")]
	public async Task<IActionResult> GetRoleByIdAsync(string roleId, CancellationToken cancel = default)
	{
		var response = await _IdentityApi.GetRoleByIdAsync(roleId, cancel);
		return Ok(response);
	}

	[HttpPut(AuthApiRoute.EditRoleNameById)]
	public async Task<IActionResult> EditRoleNameByIdAsync([FromBody] EditRoleNameByIdRequest editRoleNameByIdRequest, CancellationToken cancel = default)
	{
		var response = await _IdentityApi.EditRoleNameByIdAsync(editRoleNameByIdRequest, cancel);
		return Ok(response.Message);
	}

	[HttpDelete($"{AuthApiRoute.DeleteRoleById}/{{roleId}}")]
	public async Task<IActionResult> DeleteRoleByIdAsync(string roleId, CancellationToken cancel = default)
	{
		var response = await _IdentityApi.DeleteRoleByIdAsync(roleId, cancel);
		return Ok(response.Message);
	}

	[HttpPost(AuthApiRoute.AddRoleToUser)]
	public async Task<IActionResult> AddRoleToUserAsync([FromBody] AddRoleToUserRequest? addRoleRequest, CancellationToken cancel = default)
	{
		var response = await _IdentityApi.AddRoleToUserAsync(addRoleRequest, cancel);
		return Ok(response);
	}

	[HttpDelete($"{AuthApiRoute.DeleteUserRoleByEmail}/{{email}}/{{RoleName}}")]
	public async Task<IActionResult> DeleteUserRoleByEmailAsync(string email, string roleName, CancellationToken cancel = default)
	{
		var response = await _IdentityApi.DeleteUserRoleByEmailAsync(email, roleName, cancel);
		return Ok(response.Message);
	}

	[HttpGet($"{AuthApiRoute.GetAllUserRolesByEmail}/{{email}}")]
	public async Task<IActionResult> GetAllUserRolesByEmailAsync(string email, CancellationToken cancel = default)
	{
		var response = await _IdentityApi.GetUserRolesAsync(email, cancel);
		return Ok(response);
	}

	[HttpGet($"{AuthApiRoute.GetUserByEmail}/{{email}}")]
	public async Task<IActionResult> GetUserByEmailAsync(string email, CancellationToken cancel = default)
	{
		var response = await _IdentityApi.GetUserByEmailAsync(email, cancel);
		return Ok(response);
	}

	[HttpGet(AuthApiRoute.GetAllUsers)]
	public async Task<IActionResult> GetAllUsersAsync(CancellationToken cancel = default)
	{
		var response = await _IdentityApi.GetAllUsersAsync(cancel);
		return Ok(response);
	}

	[HttpPut(AuthApiRoute.EditUserByEmail)]
	public async Task<IActionResult> EditUserNameByEmailAsync([FromBody] EditUserNameByEmailRequest? editUserRequest, CancellationToken cancel = default)
	{
		var response = await _IdentityApi.EditUserNameByEmailAsync(editUserRequest, cancel);
		return Ok(response);
	}

	[HttpDelete($"{AuthApiRoute.DeleteUserByEmail}/{{email}}")]
	public async Task<IActionResult> DeleteUserByEmailAsync(string email, CancellationToken cancel = default)
	{
		var response = await _IdentityApi.DeleteUserByEmailAsync(email, cancel);
		return Ok(response.Message);
	}

	[HttpGet(AuthApiRoute.RefreshToken)]
	public async Task<IActionResult> GetRefreshTokenAsync(CancellationToken cancel = default)
	{
		var response = await _IdentityApi.GetRefreshTokenAsync(cancel);
		return Ok(response.Data?.RefreshToken);
	}
}