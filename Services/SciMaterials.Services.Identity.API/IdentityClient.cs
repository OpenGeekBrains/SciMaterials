using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

using Microsoft.Extensions.Logging;

using SciMaterials.Contracts;
using SciMaterials.Contracts.Identity.API;
using SciMaterials.Contracts.Identity.API.Requests.Roles;
using SciMaterials.Contracts.Identity.API.Requests.Users;
using SciMaterials.Contracts.Identity.API.Responses.DTO;
using SciMaterials.Contracts.Identity.API.Responses.User;
using SciMaterials.Contracts.Result;

namespace SciMaterials.Services.Identity.API;

/// <summary> Имплементация зеркала <see cref="IIdentityApi"/> для общения с сервером на стороне клиента операций </summary>
public class IdentityClient : IIdentityApi
{
    private readonly HttpClient _Client;
    private readonly JsonSerializerOptions _Options;
    private readonly ILogger<IdentityClient> _Logger;

    private const string ApiRoot = $"{AuthApiRoute.AuthControllerName}/";

    public IdentityClient(HttpClient Client, ILogger<IdentityClient> Logger)
    {
        _Client = Client;
        _Options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        _Logger = Logger;
    }
    
    // TODO: Убрать, это не задача клиента устанавливать токен авторизации при запросе, к тому же эта операция не конкурентна
    private static AuthenticationHeaderValue? AuthorizationHeader { get; set; }

    public async Task<Result<RegisterUserResponse>> RegisterUserAsync(RegisterRequest RegisterRequest, CancellationToken Cancel = default)
    {
        _Logger.Log(LogLevel.Information, "RegisterUser {Email}", RegisterRequest.Email);
        var content = JsonSerializer.Serialize(RegisterRequest, _Options);
        var body_content = new StringContent(content, Encoding.UTF8, "application/json");

        var registration_result = await _Client.PostAsync($"{ApiRoot}{AuthApiRoute.Register}", body_content, Cancel);
        var result = await registration_result.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<Result<RegisterUserResponse>>(cancellationToken: Cancel);

        return result ?? Result<RegisterUserResponse>.Failure(Errors.App.ParseFailure);
    }
    
    public async Task<Result<LoginUserResponse>> LoginUserAsync(LoginRequest loginRegister, CancellationToken Cancel = default)
    {
        _Logger.Log(LogLevel.Information, "LoginUser {Email}", loginRegister.Email);
        var content = JsonSerializer.Serialize(loginRegister, _Options);
        var body_content = new StringContent(content, Encoding.UTF8, "application/json");

        var login_result = await _Client.PostAsync($"{ApiRoot}{AuthApiRoute.Login}", body_content, Cancel);
        var result = await login_result.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<Result<LoginUserResponse>>(cancellationToken: Cancel);

        //Прописываем токен для будущих запросов к api, пока реализовано так.
        if (result?.Succeeded is true)
        {
            AuthorizationHeader = new AuthenticationHeaderValue("Bearer", result.Data!.SessionToken);
        }
        return result ?? Result<LoginUserResponse>.Failure(Errors.App.ParseFailure);
    }
    
    public async Task<Result> LogoutUserAsync(CancellationToken Cancel = default)
    {
        _Logger.Log(LogLevel.Information, "LogoutUser");

        var logout_result = await _Client.PostAsync($"{ApiRoot}{AuthApiRoute.Logout}", null, Cancel);
        var result = await logout_result.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<Result>(cancellationToken: Cancel);
        //_Api.DefaultRequestHeaders.Authorization = null;
        if (result?.Succeeded is true)
        {
            AuthorizationHeader = null;
        }
        return result ?? Result.Failure(Errors.App.ParseFailure);
    }
    
    public async Task<Result> ChangePasswordAsync(ChangePasswordRequest ChangePasswordRequest, CancellationToken Cancel = default)
    {
        _Logger.Log(LogLevel.Information, "ChangePassword");
        var content = JsonSerializer.Serialize(ChangePasswordRequest, _Options);
        var body_content = new StringContent(content, Encoding.UTF8, "application/json");

        _Client.DefaultRequestHeaders.Authorization = AuthorizationHeader;

        var change_password_result = await _Client.PostAsync($"{ApiRoot}{AuthApiRoute.ChangePassword}", body_content, Cancel);
        var result = await change_password_result.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<Result>(cancellationToken: Cancel);

        return result ?? Result.Failure(Errors.App.ParseFailure);
    }
    
    public async Task<Result> CreateRoleAsync(CreateRoleRequest CreateRoleRequest, CancellationToken Cancel = default)
    {
        _Logger.Log(LogLevel.Information, "CreateRole {RoleName}", CreateRoleRequest.RoleName);
        var content = JsonSerializer.Serialize(CreateRoleRequest, _Options);
        var body_content = new StringContent(content, Encoding.UTF8, "application/json");

        _Client.DefaultRequestHeaders.Authorization = AuthorizationHeader;

        var create_role_result = await _Client.PostAsync($"{ApiRoot}{AuthApiRoute.CreateRole}", body_content, Cancel);
        var result = await create_role_result.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<Result>(cancellationToken: Cancel);

        return result ?? Result.Failure(Errors.App.ParseFailure);
    }
    
    public async Task<Result<IEnumerable<AuthRole>>> GetAllRolesAsync(CancellationToken Cancel = default)
    {
        _Logger.Log(LogLevel.Information, "GetAllRoles");

        _Client.DefaultRequestHeaders.Authorization = AuthorizationHeader;

        var get_all_roles_result = await _Client.GetAsync($"{ApiRoot}{AuthApiRoute.GetAllRoles}", Cancel);
        var result = await get_all_roles_result.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<Result<IEnumerable<AuthRole>>>(cancellationToken: Cancel);

        return result ?? Result<IEnumerable<AuthRole>>.Failure(Errors.App.ParseFailure);
    }
    
    public async Task<Result<AuthRole>> GetRoleByIdAsync(string RoleId, CancellationToken Cancel = default)
    {
        _Logger.Log(LogLevel.Information, "GetRoleById {RoleId}", RoleId);

        _Client.DefaultRequestHeaders.Authorization = AuthorizationHeader;

        var getRolesByIdResult = await _Client.GetAsync($"{ApiRoot}{AuthApiRoute.GetRoleById}/{RoleId}", Cancel);
        var result = await getRolesByIdResult.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<Result<AuthRole>>(cancellationToken: Cancel);

        return result ?? Result<AuthRole>.Failure(Errors.App.ParseFailure);
    }
    
    public async Task<Result> EditRoleNameByIdAsync(EditRoleNameByIdRequest roleRequest, CancellationToken Cancel = default)
    {
        _Logger.Log(LogLevel.Information, "EditRoleById {RoleId}", roleRequest.RoleId);
        var content = JsonSerializer.Serialize(roleRequest, _Options);
        var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

        _Client.DefaultRequestHeaders.Authorization = AuthorizationHeader;

        var getRoleByIdResult = await _Client.PutAsync($"{ApiRoot}{AuthApiRoute.EditRoleNameById}", bodyContent, Cancel);
        var result = await getRoleByIdResult.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<Result>(cancellationToken: Cancel);

        return result ?? Result.Failure(Errors.App.ParseFailure);
    }
    
    public async Task<Result> DeleteRoleByIdAsync(string RoleId, CancellationToken Cancel = default)
    {
        _Logger.Log(LogLevel.Information, "DeleteRoleById {RoleId}", RoleId);

        _Client.DefaultRequestHeaders.Authorization = AuthorizationHeader;

        var deleteRoleResult = await _Client.DeleteAsync($"{ApiRoot}{AuthApiRoute.DeleteRoleById}/{RoleId}", Cancel);
        var result = await deleteRoleResult.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<Result>(cancellationToken: Cancel);

        return result ?? Result.Failure(Errors.App.ParseFailure);
    }
    
    public async Task<Result> AddRoleToUserAsync(AddRoleToUserRequest roleRequest, CancellationToken Cancel = default)
    {
        _Logger.Log(LogLevel.Information, "AddRole {Role} ToUser {Email}", roleRequest.RoleName, roleRequest.Email);
        var content = JsonSerializer.Serialize(roleRequest, _Options);
        var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

        _Client.DefaultRequestHeaders.Authorization = AuthorizationHeader;

        var addRoleToUserResult = await _Client.PostAsync($"{ApiRoot}{AuthApiRoute.AddRoleToUser}", bodyContent, Cancel);
        var result = await addRoleToUserResult.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<Result>(cancellationToken: Cancel);

        return result ?? Result.Failure(Errors.App.ParseFailure);
    }
    
    public async Task<Result> DeleteUserRoleByEmailAsync(string Email, string RoleName, CancellationToken Cancel = default)
    {
        _Logger.Log(LogLevel.Information, "DeleteUserRoleByEmail {Email}", Email);

        _Client.DefaultRequestHeaders.Authorization = AuthorizationHeader;

        var deleteUserRoleResult = await _Client.DeleteAsync($"{ApiRoot}{AuthApiRoute.DeleteUserRoleByEmail}/{Email}/{RoleName}", Cancel);
        var result = await deleteUserRoleResult.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<Result>(cancellationToken: Cancel);

        return result ?? Result.Failure(Errors.App.ParseFailure);
    }
    
    public async Task<Result<IEnumerable<AuthRole>>> GetUserRolesAsync(string Email, CancellationToken Cancel = default)
    {
        _Logger.Log(LogLevel.Information, "ListOfUser {Email} Roles", Email);

        _Client.DefaultRequestHeaders.Authorization = AuthorizationHeader;

        var listOfUserRolesResult = await _Client.GetAsync($"{ApiRoot}{AuthApiRoute.GetAllUserRolesByEmail}/{Email}", Cancel);
        var result = await listOfUserRolesResult.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<Result<IEnumerable<AuthRole>>>(cancellationToken: Cancel);

        return result ?? Result<IEnumerable<AuthRole>>.Failure(Errors.App.ParseFailure);
    }
    
    public async Task<Result<AuthUser>> GetUserByEmailAsync(string Email, CancellationToken Cancel = default)
    {
        _Logger.Log(LogLevel.Information, "GetUserByEmail {Email}", Email);

        _Client.DefaultRequestHeaders.Authorization = AuthorizationHeader;

        var getUserByEmailResult = await _Client.GetAsync($"{ApiRoot}{AuthApiRoute.GetUserByEmail}/{Email}", Cancel);
        var result = await getUserByEmailResult.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<Result<AuthUser>>(cancellationToken: Cancel);

        return result ?? Result<AuthUser>.Failure(Errors.App.ParseFailure);
    }
    
    public async Task<Result<IEnumerable<AuthUser>>> GetAllUsersAsync(CancellationToken Cancel = default)
    {
        _Logger.Log(LogLevel.Information, "GetAllUsers");

        _Client.DefaultRequestHeaders.Authorization = AuthorizationHeader;

        var getAllUsersResult = await _Client.GetAsync($"{ApiRoot}{AuthApiRoute.GetAllUsers}", Cancel);
        var result = await getAllUsersResult.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<Result<IEnumerable<AuthUser>>>(cancellationToken: Cancel);

        return result ?? Result<IEnumerable<AuthUser>>.Failure(Errors.App.ParseFailure);
    }
    
    public async Task<Result<EditUserNameResponse>> EditUserNameByEmailAsync(EditUserNameByEmailRequest editUserNameByEmailRequest, CancellationToken Cancel = default)
    {
        _Logger.Log(
            LogLevel.Information,
            "EditUserName {NickName} ByEmail {UserEmail}",
            editUserNameByEmailRequest.EditUserNickName,
            editUserNameByEmailRequest.UserEmail);

        var content = JsonSerializer.Serialize(editUserNameByEmailRequest, _Options);
        var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

        _Client.DefaultRequestHeaders.Authorization = AuthorizationHeader;

        var getUserByEmailResult = await _Client.PutAsync($"{ApiRoot}{AuthApiRoute.EditUserByEmail}", bodyContent, Cancel);
        var result = await getUserByEmailResult.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<Result<EditUserNameResponse>>(cancellationToken: Cancel);

        return result ?? Result<EditUserNameResponse>.Failure(Errors.App.ParseFailure);
    }
    
    public async Task<Result> DeleteUserByEmailAsync(string Email, CancellationToken Cancel = default)
    {
        _Logger.Log(LogLevel.Information, "DeleteUserByEmail {Email}", Email);

        _Client.DefaultRequestHeaders.Authorization = AuthorizationHeader;

        var getUserByEmailResult = await _Client.DeleteAsync($"{ApiRoot}{AuthApiRoute.DeleteUserByEmail}/{Email}", Cancel);
        var result = await getUserByEmailResult.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<Result>(cancellationToken: Cancel);

        return result ?? Result.Failure(Errors.App.ParseFailure);
    }

    public async Task<Result<RefreshTokenResponse>> GetRefreshTokenAsync(CancellationToken Cancel = default)
    {
        _Logger.Log(LogLevel.Information, "RefreshTokenAsync");

        _Client.DefaultRequestHeaders.Authorization = AuthorizationHeader;

        var refresh_token_result = await _Client.GetAsync($"{ApiRoot}{AuthApiRoute.RefreshToken}", Cancel);
        var result = await refresh_token_result.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<Result<RefreshTokenResponse>>(cancellationToken: Cancel);

        return result ?? Result<RefreshTokenResponse>.Failure(Errors.App.ParseFailure);
    }
}
