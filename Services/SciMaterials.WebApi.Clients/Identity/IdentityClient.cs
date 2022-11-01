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
    /// <param name="CancellationToken">Токен отмены</param>
    /// <returns>IdentityClientResponse</returns>
    public async Task<ClientCreateUserResponse> RegisterUserAsync(RegisterRequest RegisterRequest, CancellationToken CancellationToken = default)
    {
        _logger.Log(LogLevel.Information, "RegisterUser {Email}", RegisterRequest.Email);
        var content = JsonSerializer.Serialize(RegisterRequest, _options);
        var body_content = new StringContent(content, Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"{_client.BaseAddress}" + 
                                 $"{AuthApiRoute.AuthControllerName}/" + 
                                 $"{AuthApiRoute.Register}"),
            Content = body_content,
        };
        
        var registration_result = await _client.SendAsync(request, CancellationToken);
        var result = await registration_result.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<ClientCreateUserResponse>(cancellationToken: CancellationToken);
        return result;
    }

    /// <summary>
    /// Метод клиента для авторизации пользователя в Identity
    /// </summary>
    /// <param name="loginRegister">Запрос на авторизацию</param>
    /// <param name="Cancel">Токен отмены</param>
    /// <returns>IdentityClientResponse</returns>
    public async Task<ClientLoginResponse> LoginUserAsync(LoginRequest loginRegister, CancellationToken CancellationToken = default)
    {
        _logger.Log(LogLevel.Information, "LoginUser {Email}", loginRegister.Email);
        var content = JsonSerializer.Serialize(loginRegister, _options);
        var body_content = new StringContent(content, Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"{_client.BaseAddress}" + 
                                 $"{AuthApiRoute.AuthControllerName}/" + 
                                 $"{AuthApiRoute.Login}"),
            Content = body_content
        };
        
        var login_result = await _client.SendAsync(request, CancellationToken);
        var result = await login_result.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<ClientLoginResponse>(cancellationToken: CancellationToken);
        
        //Прописываем токен для будущих запросов к api, пока реализовано так.
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",result.SessionToken);
        return result;
    }

    /// <summary>
    /// Метод клиента для выхода пользователя из системы Identity
    /// </summary>
    /// <param name="Cancel">Токен отмены</param>
    /// <returns>IdentityClientResponse</returns>
    public async Task<ClientLogoutResponse> LogoutUserAsync(CancellationToken CancellationToken = default)
    {
        _logger.Log(LogLevel.Information, "LogoutUser");
        var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"{_client.BaseAddress}" + 
                                 $"{AuthApiRoute.AuthControllerName}/" + 
                                 $"{AuthApiRoute.Logout}"),
        };
        
        var logout_result = await _client.SendAsync(request, CancellationToken);
        var result = await logout_result.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<ClientLogoutResponse>(cancellationToken: CancellationToken);
        _client.DefaultRequestHeaders.Authorization = null;
        return result;
    }

    /// <summary>
    /// Метод клиента для смены пароля в Identity
    /// </summary>
    /// <param name="ChangePasswordRequest">Запрос на смену пароля</param>
    /// <param name="Cancel">Токен отмены</param>
    /// <returns>IdentityClientResponse</returns>
    public async Task<ClientChangePasswordResponse> ChangePasswordAsync(ChangePasswordRequest ChangePasswordRequest, CancellationToken CancellationToken = default)
    {
        _logger.Log(LogLevel.Information, "ChangePassword");
        var content = JsonSerializer.Serialize(ChangePasswordRequest, _options);
        var body_content = new StringContent(content, Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"{_client.BaseAddress}" + 
                                 $"{AuthApiRoute.AuthControllerName}/" + 
                                 $"{AuthApiRoute.ChangePassword}"),
            Content = body_content
        };
        
        var change_password_result = await _client.SendAsync(request, CancellationToken);
        var result = await change_password_result.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<ClientChangePasswordResponse>(cancellationToken: CancellationToken);
        return result;
    }

    /// <summary>
    /// Метод клиента для создания роли пользователя в Identity
    /// </summary>
    /// <param name="CreateRoleRequest">Запрос на создание роли</param>
    /// <param name="Cancel">Токен отмены</param>
    /// <returns>IdentityClientResponse</returns>
    public async Task<ClientCreateRoleResponse> CreateRoleAsync(CreateRoleRequest CreateRoleRequest, CancellationToken CancellationToken = default)
    {
        _logger.Log(LogLevel.Information, "CreateRole {RoleName}", CreateRoleRequest.RoleName);
        var content = JsonSerializer.Serialize(CreateRoleRequest, _options);
        var body_content = new StringContent(content, Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"{_client.BaseAddress}" + 
                                 $"{AuthApiRoute.AuthControllerName}/" + 
                                 $"{AuthApiRoute.CreateRole}"),
            Content = body_content
        };
        
        var create_role_result = await _client.SendAsync(request, CancellationToken);
        var result = await create_role_result.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<ClientCreateRoleResponse>(cancellationToken: CancellationToken);
        return result;
    }
    
    /// <summary>
    /// Метод клиента для получения инф. о всех ролях в Identity
    /// </summary>
    /// <param name="Cancel">Токен отмены</param>
    /// <returns>IdentityClientResponse</returns>
    public async Task<ClientGetAllRolesResponse> GetAllRolesAsync(CancellationToken CancellationToken = default)
    {
        _logger.Log(LogLevel.Information, "GetAllRoles");
        var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"{_client.BaseAddress}" + 
                                 $"{AuthApiRoute.AuthControllerName}/" + 
                                 $"{AuthApiRoute.GetAllRoles}"),
        };
        
        var get_all_roles_result = await _client.SendAsync(request, CancellationToken);
        var result = await get_all_roles_result.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<ClientGetAllRolesResponse>(cancellationToken: CancellationToken);
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
        var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"{_client.BaseAddress}" + 
                                 $"{AuthApiRoute.AuthControllerName}/" + 
                                 $"{AuthApiRoute.GetRoleById}/"+ 
                                 $"{RoleId}"),
        };

        var getRolesByIdResult = await _client.SendAsync(request, Cancel);
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
        var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Put,
            RequestUri = new Uri($"{_client.BaseAddress}" + 
                                 $"{AuthApiRoute.AuthControllerName}/" + 
                                 $"{AuthApiRoute.EditRoleNameById}"),
            Content = bodyContent
        };
        
        var getRoleByIdResult = await _client.SendAsync(request, Cancel);
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
        var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri($"{_client.BaseAddress}" + 
                                 $"{AuthApiRoute.AuthControllerName}/" + 
                                 $"{AuthApiRoute.DeleteRoleById}/" + 
                                 $"{RoleId}"),
        };
        
        var deleteRoleResult = await _client.SendAsync(request, Cancel);
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
        var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"{_client.BaseAddress}" + 
                                 $"{AuthApiRoute.AuthControllerName}/" + 
                                 $"{AuthApiRoute.AddRoleToUser}"),
            Content = bodyContent
        };
        
        var addRoleToUserResult = await _client.SendAsync(request, Cancel);
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
        var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri($"{_client.BaseAddress}" + 
                                 $"{AuthApiRoute.AuthControllerName}/" + 
                                 $"{AuthApiRoute.DeleteUserRoleByEmail}/" + 
                                 $"{Email}/" + 
                                 $"{RoleName}"),
        };
        
        var deleteUserRoleResult = await _client.SendAsync(request, Cancel);
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
        var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"{_client.BaseAddress}" + 
                                 $"{AuthApiRoute.AuthControllerName}/" + 
                                 $"{AuthApiRoute.GetAllUserRolesByEmail}/" + 
                                 $"{Email}"),
        };
        
        var listOfUserRolesResult = await _client.SendAsync(request, Cancel);
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
        var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"{_client.BaseAddress}" + 
                                 $"{AuthApiRoute.AuthControllerName}/" + 
                                 $"{AuthApiRoute.CreateUser}"),
            Content = bodyContent
        };
        
        var createUserResult = await _client.SendAsync(request, Cancel);
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
        var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"{_client.BaseAddress}" + 
                                 $"{AuthApiRoute.AuthControllerName}/" + 
                                 $"{AuthApiRoute.GetUserByEmail}/" + 
                                 $"{Email}"),
        };
        
        var getUserByEmailResult = await _client.SendAsync(request, Cancel);
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
        var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"{_client.BaseAddress}" + 
                                 $"{AuthApiRoute.AuthControllerName}/" + 
                                 $"{AuthApiRoute.GetAllUsers}"),
        };
        
        var getAllUsersResult = await _client.SendAsync(request, Cancel);
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
        var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Put,
            RequestUri = new Uri($"{_client.BaseAddress}" + 
                                 $"{AuthApiRoute.AuthControllerName}/" + 
                                 $"{AuthApiRoute.EditUserByEmail}"),
            Content = bodyContent
        };
        
        var getUserByEmailResult = await _client.SendAsync(request, Cancel);
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
        var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri($"{_client.BaseAddress}" + 
                                 $"{AuthApiRoute.AuthControllerName}/" + 
                                 $"{AuthApiRoute.DeleteUserByEmail}/" + 
                                 $"{Email}"),
        };
        
        var getUserByEmailResult = await _client.SendAsync(request, Cancel);
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
        var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri($"{_client.BaseAddress}" + 
                                 $"{AuthApiRoute.AuthControllerName}/" + 
                                 $"{AuthApiRoute.DeleteUserWithoutConfirm}"),
        };
        
        var getUserByEmailResult = await _client.SendAsync(request, Cancel);
        var result = await getUserByEmailResult.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<ClientDeleteUsersWithOutConfirmResponse>(cancellationToken: Cancel);
        return result;
    }

    public async Task<ClientRefreshTokenResponse> GetRefreshTokenAsync(CancellationToken Cancel = default)
    {
        _logger.Log(LogLevel.Information, "RefreshTokenAsync");
        var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"{_client.BaseAddress}" + 
                                 $"{AuthApiRoute.AuthControllerName}/" + 
                                 $"{AuthApiRoute.RefreshToken}"),
        };
        
        var refresh_token_result = await _client.SendAsync(request, Cancel);
        var result = await refresh_token_result.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<ClientRefreshTokenResponse>(cancellationToken: Cancel);
        return result;
    }
}