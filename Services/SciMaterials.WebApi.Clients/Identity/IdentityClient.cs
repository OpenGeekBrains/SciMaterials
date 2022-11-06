using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.API.Constants;
using SciMaterials.Contracts.Identity.API.DTO.Roles;
using SciMaterials.Contracts.Identity.API.DTO.Users;
using SciMaterials.Contracts.Identity.Clients.Clients;
using SciMaterials.Contracts.Identity.Clients.Clients.Responses.Roles;
using SciMaterials.Contracts.Identity.Clients.Clients.Responses.User;

namespace SciMaterials.WebApi.Clients.Identity;

public class IdentityClient : IIdentityClient
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _options;
    private readonly ILogger<IdentityClient> _logger;
    private static AuthenticationHeaderValue _DefaultRequestHeader;

    public IdentityClient(HttpClient client, ILogger<IdentityClient> logger)
    {
        _client = client;
        _options = new JsonSerializerOptions {PropertyNameCaseInsensitive = true};
        _logger = logger;
    }

    /// <summary>
    /// Метод клиента для регистрации пользователя в Identity
    /// </summary>
    /// <param name="RegisterRequest">Запрос на регистрацию</param>
    /// <param name="Cancel">Токен отмены</param>
    /// <returns>IdentityClientResponse</returns>
    public async Task<ClientCreateUserResponse> RegisterUserAsync(RegisterRequest RegisterRequest, CancellationToken Cancel = default)
    {
        _logger.Log(LogLevel.Information, "RegisterUser {Email}", RegisterRequest.Email);
        var content = JsonSerializer.Serialize(RegisterRequest, _options);
        var body_content = new StringContent(content, Encoding.UTF8, "application/json");
        
        var registration_result = 
            await _client.PostAsync($"{_client.BaseAddress}" + 
                                    $"{AuthApiRoute.AuthControllerName}/" + 
                                    $"{AuthApiRoute.Register}", body_content, Cancel);
        var result = await registration_result.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<ClientCreateUserResponse>(cancellationToken: Cancel);
        return result;
    }

    /// <summary>
    /// Метод клиента для авторизации пользователя в Identity
    /// </summary>
    /// <param name="loginRegister">Запрос на авторизацию</param>
    /// <param name="Cancel">Токен отмены</param>
    /// <returns>IdentityClientResponse</returns>
    public async Task<ClientLoginResponse> LoginUserAsync(LoginRequest loginRegister, CancellationToken Cancel = default)
    {
        _logger.Log(LogLevel.Information, "LoginUser {Email}", loginRegister.Email);
        var content = JsonSerializer.Serialize(loginRegister, _options);
        var body_content = new StringContent(content, Encoding.UTF8, "application/json");

        var login_result =
            await _client.PostAsync($"{_client.BaseAddress}" +
                                    $"{AuthApiRoute.AuthControllerName}/" +
                                    $"{AuthApiRoute.Login}", body_content, Cancel);
        var result = await login_result.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<ClientLoginResponse>(cancellationToken: Cancel);

        //Прописываем токен для будущих запросов к api, пока реализовано так.
        //_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.SessionToken);
        _DefaultRequestHeader = new AuthenticationHeaderValue("Bearer", result.SessionToken);
        return result;
    }

    /// <summary>
    /// Метод клиента для выхода пользователя из системы Identity
    /// </summary>
    /// <param name="Cancel">Токен отмены</param>
    /// <returns>IdentityClientResponse</returns>
    public async Task<ClientLogoutResponse> LogoutUserAsync(CancellationToken Cancel = default)
    {
        _logger.Log(LogLevel.Information, "LogoutUser");
        
        var logout_result = 
            await _client.PostAsync(
                $"{_client.BaseAddress}" + 
                $"{AuthApiRoute.AuthControllerName}/" + 
                $"{AuthApiRoute.Logout}",  null, Cancel);
        var result = await logout_result.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<ClientLogoutResponse>(cancellationToken: Cancel);
        //_client.DefaultRequestHeaders.Authorization = null;
        _DefaultRequestHeader = null;
        return result;
    }

    /// <summary>
    /// Метод клиента для смены пароля в Identity
    /// </summary>
    /// <param name="ChangePasswordRequest">Запрос на смену пароля</param>
    /// <param name="Cancel">Токен отмены</param>
    /// <returns>IdentityClientResponse</returns>
    public async Task<ClientChangePasswordResponse> ChangePasswordAsync(ChangePasswordRequest ChangePasswordRequest, CancellationToken Cancel = default)
    {
        _logger.Log(LogLevel.Information, "ChangePassword");
        var content = JsonSerializer.Serialize(ChangePasswordRequest, _options);
        var body_content = new StringContent(content, Encoding.UTF8, "application/json");

        _client.DefaultRequestHeaders.Authorization = _DefaultRequestHeader;
        var change_password_result = 
            await _client.PostAsync(
                $"{_client.BaseAddress}" + 
                $"{AuthApiRoute.AuthControllerName}/" + 
                $"{AuthApiRoute.ChangePassword}", body_content, Cancel);
        var result = await change_password_result.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<ClientChangePasswordResponse>(cancellationToken: Cancel);
        return result;
    }

    /// <summary>
    /// Метод клиента для создания роли пользователя в Identity
    /// </summary>
    /// <param name="CreateRoleRequest">Запрос на создание роли</param>
    /// <param name="Cancel">Токен отмены</param>
    /// <returns>IdentityClientResponse</returns>
    public async Task<ClientCreateRoleResponse> CreateRoleAsync(CreateRoleRequest CreateRoleRequest, CancellationToken Cancel = default)
    {
        _logger.Log(LogLevel.Information, "CreateRole {RoleName}", CreateRoleRequest.RoleName);
        var content = JsonSerializer.Serialize(CreateRoleRequest, _options);
        var body_content = new StringContent(content, Encoding.UTF8, "application/json");

        _client.DefaultRequestHeaders.Authorization = _DefaultRequestHeader;
        var create_role_result = 
            await _client.PostAsync(
                $"{_client.BaseAddress}" + 
                $"{AuthApiRoute.AuthControllerName}/" + 
                $"{AuthApiRoute.CreateRole}", body_content, Cancel);
        var result = await create_role_result.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<ClientCreateRoleResponse>(cancellationToken: Cancel);
        return result;
    }
    
    /// <summary>
    /// Метод клиента для получения инф. о всех ролях в Identity
    /// </summary>
    /// <param name="Cancel">Токен отмены</param>
    /// <returns>IdentityClientResponse</returns>
    public async Task<ClientGetAllRolesResponse> GetAllRolesAsync(CancellationToken Cancel = default)
    {
        _logger.Log(LogLevel.Information, "GetAllRoles");

        _client.DefaultRequestHeaders.Authorization = _DefaultRequestHeader;
        var get_all_roles_result = 
            await _client.GetAsync(
                $"{_client.BaseAddress}" +
                $"{AuthApiRoute.AuthControllerName}/" +
                $"{AuthApiRoute.GetAllRoles}", Cancel);
        var result = await get_all_roles_result.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<ClientGetAllRolesResponse>(cancellationToken: Cancel);
        return result;
    }

    /// <summary>
    /// Метод клиента для получения инф. о роли по идентификатору в Identity
    /// </summary>
    /// <param name="roleRequest">Запрос на получение инф. о роли по идентификатору</param>
    /// <param name="Cancel">Токен отмены</param>
    /// <returns>IdentityClientResponse</returns>
    public async Task<ClientGetRoleByIdResponse> GetRoleByIdAsync(string RoleId, CancellationToken Cancel = default)
    {
        _logger.Log(LogLevel.Information, "GetRoleById {RoleId}", RoleId);

        _client.DefaultRequestHeaders.Authorization = _DefaultRequestHeader;
        var getRolesByIdResult = 
            await _client.GetAsync(
                $"{_client.BaseAddress}" + 
                $"{AuthApiRoute.AuthControllerName}/" + 
                $"{AuthApiRoute.GetRoleById}/"+ 
                $"{RoleId}", Cancel);
        var result = await getRolesByIdResult.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<ClientGetRoleByIdResponse>(cancellationToken: Cancel);
        return result;
    }

    /// <summary>
    /// Метод клиента для редактирования роли по идентификатору в Identity
    /// </summary>
    /// <param name="roleRequest">Запрос на редактирование роли по идентификатору</param>
    /// <param name="Cancel">Токен отмены</param>
    /// <returns>IdentityClientResponse</returns>
    public async Task<ClientEditRoleNameByIdResponse> EditRoleNameByIdAsync(EditRoleNameByIdRequest roleRequest, CancellationToken Cancel = default)
    {
        _logger.Log(LogLevel.Information, "EditRoleById {RoleId}", roleRequest.RoleId);
        var content = JsonSerializer.Serialize(roleRequest, _options);
        var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

        _client.DefaultRequestHeaders.Authorization = _DefaultRequestHeader;
        var getRoleByIdResult = 
            await _client.PutAsync(
                $"{_client.BaseAddress}" + 
                $"{AuthApiRoute.AuthControllerName}/" + 
                $"{AuthApiRoute.EditRoleNameById}", bodyContent, Cancel);
        var result = await getRoleByIdResult.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<ClientEditRoleNameByIdResponse>(cancellationToken: Cancel);
        return result;
    }

    /// <summary>
    /// Метод клиента на удаление роли по идентификатору в Identity
    /// </summary>
    /// <param name="roleRequest">Запрос на удаление роли по идентификатору</param>
    /// <param name="Cancel">Токен отмены</param>
    /// <returns>IdentityClientResponse</returns>
    public async Task<ClientDeleteRoleByIdResponse> DeleteRoleByIdAsync(string RoleId, CancellationToken Cancel = default)
    {
        _logger.Log(LogLevel.Information, "DeleteRoleById {RoleId}", RoleId);

        _client.DefaultRequestHeaders.Authorization = _DefaultRequestHeader;
        var deleteRoleResult = 
            await _client.DeleteAsync(
                $"{_client.BaseAddress}" + 
                $"{AuthApiRoute.AuthControllerName}/" + 
                $"{AuthApiRoute.DeleteRoleById}/" + 
                $"{RoleId}", Cancel);
        var result = await deleteRoleResult.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<ClientDeleteRoleByIdResponse>(cancellationToken: Cancel);
        return result;
    }

    /// <summary>
    /// Метод клиента для добавления роли к пользователю в Identity
    /// </summary>
    /// <param name="roleRequest">Запрос на добавление роли</param>
    /// <param name="Cancel">Токен отмены</param>
    /// <returns>IdentityClientResponse</returns>
    public async Task<ClientAddRoleToUserResponse> AddRoleToUserAsync(AddRoleToUserRequest roleRequest, CancellationToken Cancel = default)
    {
        _logger.Log(LogLevel.Information, "AddRole {Role} ToUser {Email}", roleRequest.RoleName, roleRequest.Email);
        var content = JsonSerializer.Serialize(roleRequest, _options);
        var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

        _client.DefaultRequestHeaders.Authorization = _DefaultRequestHeader;
        var addRoleToUserResult = 
            await _client.PostAsync(
                $"{_client.BaseAddress}" + 
                $"{AuthApiRoute.AuthControllerName}/" + 
                $"{AuthApiRoute.AddRoleToUser}", bodyContent, Cancel);
        var result = await addRoleToUserResult.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<ClientAddRoleToUserResponse>(cancellationToken: Cancel);
        return result;
    }
    
    /// <summary>
    /// Метод клиента для удаления роли пользователя по email в Identity
    /// </summary>
    /// <param name="roleRequest">Запрос на удаление роли по email</param>
    /// <param name="Cancel">Токен отмены</param>
    /// <returns>IdentityClientResponse</returns>
    public async Task<ClientDeleteUserRoleByEmailResponse> DeleteUserRoleByEmailAsync(string Email, string RoleName, CancellationToken Cancel = default)
    {
        _logger.Log(LogLevel.Information, "DeleteUserRoleByEmail {Email}", Email);

        _client.DefaultRequestHeaders.Authorization = _DefaultRequestHeader;
        var deleteUserRoleResult = 
            await _client.DeleteAsync(
                $"{_client.BaseAddress}" + 
                $"{AuthApiRoute.AuthControllerName}/" + 
                $"{AuthApiRoute.DeleteUserRoleByEmail}/" + 
                $"{Email}/" + 
                $"{RoleName}", Cancel);
        var result = await deleteUserRoleResult.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<ClientDeleteUserRoleByEmailResponse>(cancellationToken: Cancel);
        return result;
    }

    /// <summary>
    /// Метод клиента для получения информации о всех ролях в системе в Identity
    /// </summary>
    /// <param name="roleRequest">Запрос на получение информации о всех пользователях</param>
    /// <param name="Cancel">Токен отмены</param>
    /// <returns>IdentityClientResponse</returns>
    public async Task<ClientGetAllUserRolesByEmailResponse> GetAllUserRolesByEmailAsync(string Email, CancellationToken Cancel = default)
    {
        _logger.Log(LogLevel.Information, "ListOfUser {Email} Roles", Email);

        _client.DefaultRequestHeaders.Authorization = _DefaultRequestHeader;
        var listOfUserRolesResult = 
            await _client.GetAsync(
                $"{_client.BaseAddress}" + 
                $"{AuthApiRoute.AuthControllerName}/" + 
                $"{AuthApiRoute.GetAllUserRolesByEmail}/" + 
                $"{Email}", Cancel);
        var result = await listOfUserRolesResult.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<ClientGetAllUserRolesByEmailResponse>(cancellationToken: Cancel);
        return result;
    }
    
    /// <summary>
    /// Метод клиента для создания нового пользователя (админом) в Identity
    /// </summary>
    /// <param name="registerRequest">Запрос на создание пользователя</param>
    /// <param name="Cancel">Токен отмены</param>
    /// <returns>IdentityClientResponse</returns>
    public async Task<ClientCreateUserResponse> CreateUserAsync(RegisterRequest registerRequest, CancellationToken Cancel = default)
    {
        _logger.Log(LogLevel.Information, "CreateUser {Email} {NickName}", registerRequest.Email, registerRequest.NickName);
        var content = JsonSerializer.Serialize(registerRequest, _options);
        var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

        _client.DefaultRequestHeaders.Authorization = _DefaultRequestHeader;
        var createUserResult = 
            await _client.PostAsync(
                $"{_client.BaseAddress}" + 
                $"{AuthApiRoute.AuthControllerName}/" + 
                $"{AuthApiRoute.CreateUser}", bodyContent, Cancel);
        var result = await createUserResult.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<ClientCreateUserResponse>(cancellationToken: Cancel);
        return result;
    }

    /// <summary>
    /// Метод клиента для получения информации о пользователе по email в Identity
    /// </summary>
    /// <param name="registerRequest">Запрос на получение информации о пользователе</param>
    /// <param name="Cancel">Токен отмены</param>
    /// <returns>IdentityClientResponse</returns>
    public async Task<ClientGetUserByEmailResponse> GetUserByEmailAsync(string Email, CancellationToken Cancel = default)
    {
        _logger.Log(LogLevel.Information, "GetUserByEmail {Email}", Email);

        _client.DefaultRequestHeaders.Authorization = _DefaultRequestHeader;
        var getUserByEmailResult = 
            await _client.GetAsync(
                $"{_client.BaseAddress}" + 
                $"{AuthApiRoute.AuthControllerName}/" + 
                $"{AuthApiRoute.GetUserByEmail}/" + 
                $"{Email}", Cancel);
        var result = await getUserByEmailResult.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<ClientGetUserByEmailResponse>(cancellationToken: Cancel);
        return result;
    }

    /// <summary>
    /// Метод клиента для получения информации о всех пользователях в Identity
    /// </summary>
    /// <param name="Cancel">Токен отмены</param>
    /// <returns>IdentityClientResponse</returns>
    public async Task<ClientGetAllUsersResponse> GetAllUsersAsync(CancellationToken Cancel = default)
    {
        _logger.Log(LogLevel.Information, "GetAllUsers");

        _client.DefaultRequestHeaders.Authorization = _DefaultRequestHeader;
        var getAllUsersResult = 
            await _client.GetAsync(
                $"{_client.BaseAddress}" + 
                $"{AuthApiRoute.AuthControllerName}/" + 
                $"{AuthApiRoute.GetAllUsers}", Cancel);
        var result = await getAllUsersResult.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<ClientGetAllUsersResponse>(cancellationToken: Cancel);
        return result;
    }

    /// <summary>
    /// Метод клиента для изменения имени (ник нейма) пользователя в Identity
    /// </summary>
    /// <param name="editUserNameByEmailRequest">Запрос на изменение имени</param>
    /// <param name="Cancel">Токен отмены</param>
    /// <returns>IdentityClientResponse</returns>
    public async Task<ClientEditUserNameByEmailResponse> EditUserNameByEmailAsync(EditUserNameByEmailRequest editUserNameByEmailRequest, CancellationToken Cancel = default)
    {
        _logger.Log(LogLevel.Information, "EditUserName {NickName} ByEmail {UserEmail}", 
            editUserNameByEmailRequest.EditUserNickName, editUserNameByEmailRequest.UserEmail);
        var content = JsonSerializer.Serialize(editUserNameByEmailRequest, _options);
        var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

        _client.DefaultRequestHeaders.Authorization = _DefaultRequestHeader;
        var getUserByEmailResult = 
            await _client.PutAsync(
                $"{_client.BaseAddress}" + 
                $"{AuthApiRoute.AuthControllerName}/" + 
                $"{AuthApiRoute.EditUserByEmail}", bodyContent, Cancel);
        var result = await getUserByEmailResult.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<ClientEditUserNameByEmailResponse>(cancellationToken: Cancel);
        return result;
    }

    /// <summary>
    /// Метод клиента для удаления пользователя по email в Identity
    /// </summary>
    /// <param name="registerRequest">Запрос на удаление</param>
    /// <param name="Cancel">Токен отмены</param>
    /// <returns>IdentityClientResponse</returns>
    public async Task<ClientDeleteUserByEmailResponse> DeleteUserByEmailAsync(string Email, CancellationToken Cancel = default)
    {
        _logger.Log(LogLevel.Information, "DeleteUserByEmail {Email}", Email);

        _client.DefaultRequestHeaders.Authorization = _DefaultRequestHeader;
        var getUserByEmailResult = 
            await _client.DeleteAsync(
                $"{_client.BaseAddress}" + 
                $"{AuthApiRoute.AuthControllerName}/" + 
                $"{AuthApiRoute.DeleteUserByEmail}/" + 
                $"{Email}", Cancel);
        var result = await getUserByEmailResult.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<ClientDeleteUserByEmailResponse>(cancellationToken: Cancel);
        return result;
    }

    /// <summary>
    /// Метод клиента для удаления незарегистрированных пользователей в Identity
    /// </summary>
    /// <param name="Cancel">Токен отмены</param>
    /// <returns>IdentityClientResponse</returns>
    public async Task<ClientDeleteUsersWithOutConfirmResponse> DeleteUsersWithOutConfirmAsync(CancellationToken Cancel = default)
    {
        _logger.Log(LogLevel.Information, "DeleteUsersWithOutConfirm");

        _client.DefaultRequestHeaders.Authorization = _DefaultRequestHeader;
        var getUserByEmailResult = 
            await _client.DeleteAsync(
                $"{_client.BaseAddress}" + 
                $"{AuthApiRoute.AuthControllerName}/" + 
                $"{AuthApiRoute.DeleteUserWithoutConfirm}", Cancel);
        var result = await getUserByEmailResult.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<ClientDeleteUsersWithOutConfirmResponse>(cancellationToken: Cancel);
        return result;
    }

    public async Task<ClientRefreshTokenResponse> GetRefreshTokenAsync(CancellationToken Cancel = default)
    {
        _logger.Log(LogLevel.Information, "RefreshTokenAsync");

        _client.DefaultRequestHeaders.Authorization = _DefaultRequestHeader;
        var refresh_token_result = 
            await _client.GetAsync(
                $"{_client.BaseAddress}" + 
                $"{AuthApiRoute.AuthControllerName}/" + 
                $"{AuthApiRoute.RefreshToken}", Cancel);
        var result = await refresh_token_result.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<ClientRefreshTokenResponse>(cancellationToken: Cancel);
        return result;
    }
}