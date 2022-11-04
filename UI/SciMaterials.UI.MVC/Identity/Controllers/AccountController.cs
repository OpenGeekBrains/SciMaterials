using System.IdentityModel.Tokens.Jwt;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;

using SciMaterials.Contracts.API.Constants;
using SciMaterials.Contracts.API.DTO.AuthUsers;
using SciMaterials.Contracts.Auth;
using SciMaterials.Contracts.AuthApi.DTO.Roles;
using SciMaterials.Contracts.AuthApi.DTO.Users;
using SciMaterials.Contracts.Enums;
using SciMaterials.Contracts.Identity.API.DTO.Roles;
using SciMaterials.Contracts.Identity.Clients.Clients.Responses;
using SciMaterials.Contracts.Identity.Clients.Clients.Responses.Roles;
using SciMaterials.Contracts.Identity.Clients.Clients.Responses.User;

namespace SciMaterials.UI.MVC.Identity.Controllers;

/// <summary>
/// Контроллер для регистрации и авторизации в системе аутентификации
/// </summary>
[ApiController]
[Route(AuthApiRoute.AuthControllerName)]
public class AccountController : Controller
{
    private readonly UserManager<IdentityUser> _UserManager;
    private readonly SignInManager<IdentityUser> _SignInManager;
    private readonly RoleManager<IdentityRole> _RoleManager;
    private readonly IHttpContextAccessor _ContextAccessor;
    private readonly IAuthUtilits _authUtilits;
    private readonly ILogger<AccountController> _Logger;

    public AccountController(
        UserManager<IdentityUser> UserManager,
        SignInManager<IdentityUser> SignInManager,
        RoleManager<IdentityRole> RoleManager,
        IHttpContextAccessor ContextAccessor,
        IAuthUtilits authUtilits,
        ILogger<AccountController> Logger)
    {
        _UserManager     = UserManager;
        _SignInManager   = SignInManager;
        _RoleManager     = RoleManager;
        _Logger          = Logger;
        _ContextAccessor = ContextAccessor;
        _authUtilits     = authUtilits;
    }

    /// <summary>
    /// Метод регистрации пользователя
    /// </summary>
    /// <param name="RegisterRequest">Запрос пользователя</param>
    /// <returns>Status 200 OK.</returns>
    [AllowAnonymous]
    [HttpPost(AuthApiRoute.Register)]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest? RegisterRequest)
    {
        try
        {
            var identity_user = new IdentityUser{Email = RegisterRequest.Email, UserName = RegisterRequest.NickName};
            
            var identity_result = await _UserManager.CreateAsync(identity_user, RegisterRequest.Password);
            if (identity_result.Succeeded)
            {
                await _UserManager.AddToRoleAsync(identity_user, AuthApiRoles.User);
                //await _SignInManager.SignInAsync(identity_user, false);

                var email_confirm_token = await _UserManager.GenerateEmailConfirmationTokenAsync(identity_user);

                var callback_url = Url.Action(
                    "ConfirmEmail",
                    controller: "Account",
                    values: new { UserId = identity_user.Id, ConfirmToken = email_confirm_token }, 
                    protocol: HttpContext.Request.Scheme);

                return Ok(new ClientCreateUserResponse()
                {
                    Succeeded    = true,
                    Code         = (int)ResultCodes.Ok,
                    Message      = "Пройдите по ссылке, чтобы подтвердить ваш email",
                    ConfirmEmail = callback_url,
                });
            }

            _Logger.Log(LogLevel.Information, "Не удалось зарегистрировать пользователя {Email}",
                RegisterRequest.Email);
            return Ok(new ClientCreateUserResponse
            {
                Succeeded = false, 
                Code      = (int)ResultCodes.NotFound, 
                Message   = $"Не удалось зарегистрировать пользователя {RegisterRequest.Email}"
            });
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Пользователя не удалось зарегистрировать {Ex}", ex);
            return Ok(new ClientCreateUserResponse {Succeeded = false, Code = (int)ResultCodes.ServerError});
        }
    }

    /// <summary>
    /// Метод авторизации пользователя
    /// </summary>
    /// <param name="LoginRequest">Запрос пользователя</param>
    /// <returns>Status 200 OK.</returns>
    [AllowAnonymous]
    [HttpPost(AuthApiRoute.Login)]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest? LoginRequest)
    {
        try
        {
            var identity_user = await _UserManager.FindByEmailAsync(LoginRequest.Email);
            if (identity_user is not null)
            {
                var identity_roles = await _UserManager.GetRolesAsync(identity_user);

                var sign_in_result = await _SignInManager.PasswordSignInAsync(
                    userName: LoginRequest.Email,
                    password: LoginRequest.Password,
                    isPersistent: true,
                    lockoutOnFailure: false);

                if (sign_in_result.Succeeded)
                {
                    var session_token = _authUtilits.CreateSessionToken(identity_user, identity_roles);
                    return Ok(new ClientLoginResponse
                    {
                        Succeeded    = true,
                        Code         = (int) ResultCodes.Ok,
                        SessionToken = session_token
                    });
                }

                _Logger.Log(LogLevel.Information, "Не удалось авторизовать пользователя {Email}", LoginRequest.Email);
                return Ok(new ClientLoginResponse
                {
                    Succeeded = false, 
                    Code      = (int)ResultCodes.NotFound,
                    Message   = $"Не удалось авторизовать пользователя {LoginRequest.Email}"
                });
            }
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Не удалось авторизовать пользователя {Ex}", ex);
            return Ok(new ClientLoginResponse{Succeeded = false, Code = (int)ResultCodes.ServerError});
        }
        
        _Logger.Log(LogLevel.Information, "Некорректно введены данные {Email}, {Password}",
            LoginRequest.Email, LoginRequest.Password);
        return Ok(new ClientLoginResponse
        {
            Succeeded = false,
            Code      = (int)ResultCodes.NotFound,
            Message   = $"Некорректно введены данные"
        });
    }

    /// <summary>
    /// Метод выхода пользователя из системы
    /// </summary>
    /// <returns>Status 200 OK.</returns>
    [HttpPost(AuthApiRoute.Logout)]
    public async Task<IActionResult> LogoutAsync()
    {
        try
        {
            await _SignInManager.SignOutAsync();
            return Ok(new ClientLogoutResponse
            {
                Succeeded = true, Code = (int)ResultCodes.Ok, Message = "Пользователь вышел из системы"
            });
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Не удалось выйти из системы {Ex}", ex);
            return Ok(new ClientLogoutResponse{Succeeded = false, Code = (int)ResultCodes.ServerError});
        }
    }

    /// <summary>
    /// Метод смены пароля пользователя
    /// </summary>
    /// <param name="ChangePasswordRequest">Запрос на смену пароля</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpPost(AuthApiRoute.ChangePassword)]
    public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordRequest? ChangePasswordRequest)
    {
        try
        {
            var current_user_name = _ContextAccessor.HttpContext?.User.Identity?.Name;

            var identity_user      = await _UserManager.FindByNameAsync(current_user_name);
            var is_email_confirmed = await _UserManager.IsEmailConfirmedAsync(identity_user);
            if (current_user_name is not { Length: > 0 } || identity_user is not null || is_email_confirmed)
            {
                var identity_result = await _UserManager.ChangePasswordAsync(
                    identity_user!, ChangePasswordRequest.CurrentPassword, ChangePasswordRequest.NewPassword);
                if (identity_result.Succeeded)
                {
                    await _SignInManager.RefreshSignInAsync(identity_user);
                    
                    return Ok(new ClientChangePasswordResponse()
                    {
                        Succeeded = true,
                        Code      = (int)ResultCodes.Ok,
                        Message   = "Пароль успешно изменен",
                    });
                }

                _Logger.Log(LogLevel.Information, "Не удалось изменить пароль {CurrentPassword}, {NewPassword}",
                    ChangePasswordRequest.CurrentPassword, ChangePasswordRequest.NewPassword);
                return Ok(new ClientChangePasswordResponse() {Succeeded = false, Code = (int) ResultCodes.ValidationError});
            }

            _Logger.Log(LogLevel.Information,
                "Не удалось получить информацию о пользователе {IdentityUser} или ваша почта не подтверждена {isEmailCorfirmed}",
                identity_user, is_email_confirmed);
            return Ok(new ClientChangePasswordResponse()
            {
                Succeeded = false, 
                Code      = (int) ResultCodes.NotFound,
                Message   = $"Не удалось получить информацию о пользователе {identity_user} или ваша почта не подтверждена {is_email_confirmed}"
            });
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Произошла ошибка при смене пароля {Ex}", ex);
            return Ok(new ClientChangePasswordResponse() {Succeeded = false, Code = (int) ResultCodes.ServerError});
        }
    }
    
    /// <summary>
    /// Метод обновления токена пользователя
    /// </summary>
    /// <returns>Status 200 OK.</returns>
    [HttpGet(AuthApiRoute.RefreshToken)]
    public async Task<IActionResult> GetRefreshTokenAsync()
    {
        try
        {
            //Не обращай внимание, я тут буду править.
            var headersAuthorization = (string)_ContextAccessor.HttpContext.Request.Headers.Authorization;
            var jwtToken             = headersAuthorization.Remove(0, 7);
            
            var handler = new JwtSecurityTokenHandler();
            var token   = handler.ReadJwtToken(jwtToken);
            
            var userEmail = (string)token.Payload.First(x => x.Key.Equals("email")).Value;

            var identity_user     = await _UserManager.FindByEmailAsync(userEmail);
            var identity_roles    = await _UserManager.GetRolesAsync(identity_user);
            var new_session_token = _authUtilits.CreateSessionToken(identity_user, identity_roles);
            
            if (!string.IsNullOrEmpty(new_session_token))
            {
                return Ok(new ClientRefreshTokenResponse
                {
                    Succeeded    = true,
                    Code         = (int) ResultCodes.Ok,
                    NickName     = identity_user.UserName,
                    Email        = identity_user.Email,
                    Roles        = identity_roles,
                    RefreshToken = new_session_token
                });
            }
            
            _Logger.Log(LogLevel.Information, "Не удалось обновить токен пользователя");
            return Ok(new ClientRefreshTokenResponse
            {
                Succeeded = false, 
                Code      = (int)ResultCodes.NotFound,
                Message   = "Не удалось обновить токен пользователя"
            });
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Не удалось обновить токен пользователя {Ex}", ex);
            return Ok(new ClientRefreshTokenResponse{Succeeded = false, Code = (int)ResultCodes.ServerError});
        }
    }

    /// <summary>
    /// Метод подтверждения почты пользователя, когда пользователь проходит по сформированной ссылке
    /// </summary>
    /// <param name="UserId">Идентификатор пользователя в системе</param>
    /// <param name="ConfirmToken">Токен подтверждения</param>
    /// <returns>Status 200 OK.</returns>
    [HttpGet(AuthApiRoute.ConfirmEmail)]
    public async Task<IActionResult> ConfirmEmailAsync(string UserId, string ConfirmToken)
    {
        try
        {
            var identity_user = await _UserManager.FindByIdAsync(UserId);
            if (identity_user is not null)
            {
                var identity_result = await _UserManager.ConfirmEmailAsync(identity_user, ConfirmToken);
                if (identity_result.Succeeded)
                {
                    return Ok(new ClientConfirmEmailResponse()
                    {
                        Succeeded = true, 
                        Code      = (int) ResultCodes.Ok, 
                        Message   = $"Учетная запись {identity_user.Email} успешно подтверждена"
                    });
                }

                _Logger.Log(LogLevel.Information, "Не удалось подтвердить email пользователя");
                return Ok(new ClientConfirmEmailResponse()
                {
                    Succeeded = false, 
                    Code      = (int) ResultCodes.NotFound,
                    Message   = "Не удалось подтвердить email пользователя"
                });
            }

            _Logger.Log(LogLevel.Information, "Не удалось найти пользователя в системе {UserId}", UserId);
            return Ok(new ClientConfirmEmailResponse()
            {
                Succeeded = false, 
                Code      = (int) ResultCodes.NotFound,
                Message   = "Не удалось найти пользователя в системе"
            });
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Произошла ошибка при подтверждении почты {Ex}", ex);
            return Ok(new ClientConfirmEmailResponse() {Succeeded = false, Code = (int) ResultCodes.ServerError});
        }
    }

    /// <summary>
    /// Метод создания роли для пользователя
    /// </summary>
    /// <param name="CreateRoleRequest">Запроc на создание роли</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpPost(AuthApiRoute.CreateRole)]
    public async Task<IActionResult> CreateRoleAsync([FromBody] CreateRoleRequest CreateRoleRequest)
    {
        try
        {
            var identity_result = await _RoleManager.CreateAsync(new IdentityRole(CreateRoleRequest.RoleName.ToLower()));
            if (identity_result.Succeeded)
            {
                return Ok(new ClientCreateRoleResponse()
                {
                    Succeeded = true, 
                    Code      = (int)ResultCodes.Ok, 
                    Message   = $"Роль {CreateRoleRequest.RoleName} успешно добавлена в систему"
                });
            }

            _Logger.Log(LogLevel.Information, "Не удалось создать роль");
            return Ok(new ClientCreateRoleResponse()
            {
                Succeeded = false, 
                Code      = (int)ResultCodes.ApiError,
                Message   = $"Не удалось создать роль {CreateRoleRequest.RoleName}"
            });
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Произошла ошибка при создании роли {Ex}", ex);
            return Ok(new ClientCreateRoleResponse(){Succeeded = false, Code = (int)ResultCodes.ServerError});
        }
    }

    /// <summary>
    /// Метод получения всех ролей в системе
    /// </summary>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpGet(AuthApiRoute.GetAllRoles)]
    public async Task<IActionResult> GetAllRolesAsync()
    {
        try
        {
            var identity_roles_list = await _RoleManager.Roles.ToListAsync();
            if (identity_roles_list.Count != 0)
            {
                return Ok(new ClientGetAllRolesResponse()
                {
                    Succeeded = true, 
                    Code      = (int)ResultCodes.Ok, 
                    Roles     = identity_roles_list
                });
            }

            _Logger.Log(LogLevel.Information, "Не удалось получить список ролей");
            return Ok(new ClientGetAllRolesResponse()
            {
                Succeeded = false, 
                Code      = (int)ResultCodes.NotFound,
                Message   = "Не удалось получить список ролей"
            });
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Произошла ошибка при запросе ролей {Ex}", ex);
            return Ok(new ClientGetAllRolesResponse(){Succeeded = false, Code = (int)ResultCodes.ServerError});
        }
    }

    /// <summary>
    /// Метод получения роли по идентификатору
    /// </summary>
    /// <param name="RoleId">Идентификатор роли</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpGet($"{AuthApiRoute.GetRoleById}"+"{RoleId}")]
    public async Task<IActionResult> GetRoleByIdAsync(string RoleId)
    {
        try
        {
            var identity_role = await _RoleManager.FindByIdAsync(RoleId);
            if (identity_role is not null)
            {
                return Ok(new ClientGetRoleByIdResponse()
                {
                    Succeeded = true,
                    Code      = (int)ResultCodes.Ok,
                    Role      = identity_role
                });
            }

            _Logger.Log(LogLevel.Information, "Не удалось получить роль");
            return Ok(new ClientGetRoleByIdResponse()
            {
                Succeeded = false, 
                Code      = (int)ResultCodes.NotFound,
                Message   = "Не удалось получить роль"
            });
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Произошла ошибка при запросе роли {Ex}", ex);
            return Ok(new ClientGetRoleByIdResponse(){Succeeded = false, Code = (int)ResultCodes.ServerError});
        }
    }

    /// <summary>
    /// Метод редактирования роли по идентификатору
    /// </summary>
    /// <param name="EditRoleRequest">Запрос на редактирование роли</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpPut(AuthApiRoute.EditRoleNameById)]
    public async Task<IActionResult> EditRoleNameByIdAsync([FromBody] EditRoleNameByIdRequest? EditRoleRequest)
    {
        try
        {
            var identity_role = await _RoleManager.FindByIdAsync(EditRoleRequest.RoleId);
            identity_role.Name           = EditRoleRequest.RoleName.ToLower();
            identity_role.NormalizedName = EditRoleRequest.RoleName.ToUpper();
            
            var identity_result = await _RoleManager.UpdateAsync(identity_role);
            if (identity_result.Succeeded)
            {
                return Ok(new ClientEditRoleNameByIdResponse()
                {
                    Succeeded = true, 
                    Code      = (int)ResultCodes.Ok, 
                    Message   = $"Роль успешно изменена на {EditRoleRequest.RoleName}"
                });
            }

            _Logger.Log(LogLevel.Information, "Не удалось изменить роль");
            return Ok(new ClientEditRoleNameByIdResponse()
            {
                Succeeded = false, Code = (int)ResultCodes.NotFound, Message = "Не удалось изменить роль"
            });
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Произошла ошибка при редактировании роли {Ex}", ex);
            return Ok(new ClientEditRoleNameByIdResponse(){Succeeded = false, Code = (int)ResultCodes.ServerError});
        }
    }

    /// <summary>
    /// Метод удаления роли по идентификатору
    /// </summary>
    /// <param name="RoleId">Запрос на удаление роли</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpDelete($"{AuthApiRoute.DeleteRoleById}"+"{RoleId}")]
    public async Task<IActionResult> DeleteRoleByIdAsync(string RoleId)
    {
        try
        {
            var identity_role = await _RoleManager.FindByIdAsync(RoleId);
            if (identity_role is not null)
            {
                var identity_result = await _RoleManager.DeleteAsync(identity_role);
                if (identity_result.Succeeded)
                {
                    return Ok(new ClientDeleteRoleByIdResponse()
                    {
                        Succeeded = true, Code = (int)ResultCodes.Ok, Message = $"Роль {identity_role.Name} успешно удалена"
                    });
                }

                _Logger.Log(LogLevel.Information, "Не удалось удалить роль");
                return Ok(new ClientDeleteRoleByIdResponse()
                {
                    Succeeded = false, 
                    Code      = (int)ResultCodes.ApiError,
                    Message   = "Не удалось удалить роль"
                });
            }

            _Logger.Log(LogLevel.Information, "Не удалось найти роль");
            return Ok(new ClientDeleteRoleByIdResponse()
            {
                Succeeded = false, 
                Code      = (int)ResultCodes.NotFound,
                Message   = "Не удалось найти роль"
            });
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Произошла ошибка при удалении роли {Ex}", ex);
            return Ok(new ClientDeleteRoleByIdResponse(){Succeeded = false, Code = (int)ResultCodes.ServerError});
        }
    }

    /// <summary>
    /// Метод добавления роли к пользователю
    /// </summary>
    /// <param name="AddRoleToUserRequest">Запрос на добавление роли</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpPost(AuthApiRoute.AddRoleToUser)]
    public async Task<IActionResult> AddRoleToUserAsync([FromBody] AddRoleToUserRequest? AddRoleToUserRequest)
    {
        try
        {
            var identity_user   = await _UserManager.FindByEmailAsync(AddRoleToUserRequest.Email);
            var user_roles_list = await _UserManager.GetRolesAsync(identity_user);
            
            if (!user_roles_list.Contains(AddRoleToUserRequest.RoleName.ToLower()))
            {
                var system_roles_list = await _RoleManager.Roles.ToListAsync();
                var is_role_contains_in_system = system_roles_list
                   .Select(x => x.Name.Contains(AddRoleToUserRequest.RoleName!.ToLower()));
                foreach (var is_role in is_role_contains_in_system)
                {
                    if (is_role)
                    {
                        var role_added_result = await _UserManager.AddToRoleAsync(identity_user, AddRoleToUserRequest.RoleName!.ToLower());
                        if (role_added_result.Succeeded)
                        {
                            var new_token = _authUtilits.CreateSessionToken(identity_user,
                                await _UserManager.GetRolesAsync(identity_user));
                            
                            return Ok(new ClientAddRoleToUserResponse()
                            {
                                Succeeded = true, 
                                Code      = (int)ResultCodes.Ok,
                                NewToken  = new_token,
                                Message   = $"Роль {AddRoleToUserRequest.RoleName} успешно добавлена пользователю {identity_user.Email}"
                            });
                        }
                    }
                }
            }

            _Logger.Log(LogLevel.Information, "Некорректно введены данные или данная роль уже есть у пользователя");
            return Ok(new ClientAddRoleToUserResponse()
            {
                Succeeded = false, 
                Code      = (int)ResultCodes.NotFound,
                Message   = "Некорректно введены данные или данная роль уже есть у пользователя"
            });
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Произошла ошибка при добавлении роли к пользователю {Ex}", ex);
            return Ok(new ClientAddRoleToUserResponse(){Succeeded = false, Code = (int)ResultCodes.ServerError});
        }
    }

    /// <summary>
    /// Метод удаления роли у пользователя
    /// </summary>
    /// <param name="Email">Почта</param>
    /// <param name="RoleName">Название роли</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpDelete($"{AuthApiRoute.DeleteUserRoleByEmail}"+"{Email}/{RoleName}")]
    public async Task<IActionResult> DeleteUserRoleByEmailAsync(string Email, string RoleName)
    {
        try
        {
            var identity_user     = await _UserManager.FindByEmailAsync(Email);
            var user_roles_list   = await _UserManager.GetRolesAsync(identity_user);
            var system_roles_list = await _RoleManager.Roles.ToListAsync();
            if (user_roles_list.Contains(RoleName.ToLower()))
            {
                var is_role_contains_in_system = system_roles_list
                   .Select(x => x.Name.Contains(RoleName.ToLower()));
                foreach (var is_role in is_role_contains_in_system)
                {
                    if (is_role)
                    {
                        var role_removed_result = await _UserManager.RemoveFromRoleAsync(identity_user, RoleName.ToLower());
                        if (role_removed_result.Succeeded)
                        {
                            var new_token = _authUtilits.CreateSessionToken(identity_user,
                                await _UserManager.GetRolesAsync(identity_user));
                            
                            return Ok(new ClientDeleteUserRoleByEmailResponse()
                            {
                                Succeeded = true,
                                Code      = (int)ResultCodes.Ok,
                                NewToken  = new_token,
                                Message   = $"Роль {RoleName} успешно удалена у пользователя {identity_user.Email}"
                            });
                        }
                    }
                }
            }

            _Logger.Log(LogLevel.Information, "Некорректно введены данные {Email}, {RoleName}", 
                Email, RoleName);
            return Ok(new ClientDeleteUserRoleByEmailResponse()
            {
                Succeeded = false, 
                Code      = (int)ResultCodes.NotFound,
                Message   = $"Некорректно введены данные {Email} или {RoleName}"
            });
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Произошла ошибка при удалении роли пользователю {Ex}", ex);
            return Ok(new ClientDeleteUserRoleByEmailResponse(){Succeeded = false, Code = (int)ResultCodes.ServerError});
        }
    }

    /// <summary>
    /// Метод получения всех ролей пользователя
    /// </summary>
    /// <param name="Email">Почта</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpGet($"{AuthApiRoute.GetAllUserRolesByEmail}"+"{Email}")]
    public async Task<IActionResult> GetAllUserRolesByEmailAsync(string Email)
    {
        try
        {
            var identity_user = await _UserManager.FindByEmailAsync(Email);
            if (identity_user is not null)
            {
                var user_roles_list = await _UserManager.GetRolesAsync(identity_user);
                if (user_roles_list.Count != 0)
                {
                    return Ok(new ClientGetAllUserRolesByEmailResponse()
                    {
                        Succeeded = true, 
                        Code      = (int)ResultCodes.Ok, 
                        Roles     = user_roles_list
                    });
                }

                _Logger.Log(LogLevel.Information, "Не удалось получить список ролей");
                return Ok(new ClientGetAllUserRolesByEmailResponse()
                {
                    Succeeded = false, 
                    Code      = (int)ResultCodes.NotFound,
                    Message   = $"Не удалось получить список ролей пользователя {identity_user.Email}"
                });
            }

            _Logger.Log(LogLevel.Information,
                "Данного пользователя {IdentityUser} нет в системе, либо некорректно введены данные пользователя " +
                "{Email}", identity_user, Email);
            return Ok(new ClientGetAllUserRolesByEmailResponse()
            {
                Succeeded = false, 
                Code      = (int)ResultCodes.NotFound,
                Message   = $"Пользователя нет в системе, либо некорректно введены данные {Email}"
            });
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Произошла ошибка при получении списка ролей пользователей {Ex}", ex);
            return Ok(new ClientGetAllUserRolesByEmailResponse(){Succeeded = false, Code = (int)ResultCodes.ServerError});
        }
    }

    /// <summary>
    /// Метод создания пользователя админом
    /// </summary>
    /// <param name="CreateUserRequest">Запрос админа</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpPost(AuthApiRoute.CreateUser)]
    public async Task<IActionResult> CreateUserAsync([FromBody] RegisterRequest? CreateUserRequest)
    {
        var action_result = await RegisterAsync(CreateUserRequest);
        if (action_result is not null)
        {
            var result   = action_result as OkObjectResult;
            var response = result?.Value as ClientCreateUserResponse;
            return Ok(new ClientCreateUserResponse()
            {
                Succeeded    = response.Succeeded, 
                Code         = response.Code,
                Message      = response.Message,
                ConfirmEmail = response.ConfirmEmail
            });
        }
        
        _Logger.Log(LogLevel.Information, "Не удалось создать пользователя");
        return Ok(new ClientCreateUserResponse()
        {
            Succeeded = false, 
            Code      = (int)ResultCodes.NotFound,
            Message   = $"Не удалось создать пользователя {CreateUserRequest.Email}"
        });
    }

    /// <summary>
    /// Метод получения информации о пользователе
    /// </summary>
    /// <param name="Email">Почта</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpGet($"{AuthApiRoute.GetUserByEmail}"+"{Email}")]
    public async Task<IActionResult> GetUserByEmailAsync(string Email)
    {
        try
        {
            var identity_user = await _UserManager.FindByEmailAsync(Email);
            if (identity_user is not null)
            {
                return Ok(new ClientGetUserByEmailResponse()
                {
                    Succeeded = true, 
                    Code      = (int)ResultCodes.Ok, 
                    User      = identity_user
                });
            }

            _Logger.Log(LogLevel.Information, "Не удалось получить информации о пользователе");
            return Ok(new ClientGetUserByEmailResponse()
            {
                Succeeded = false, 
                Code      = (int)ResultCodes.NotFound,
                Message   = $"Не удалось получить информации о пользователе {Email}"
            });
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Произошла ошибка при получении пользователей {Ex}", ex);
            return Ok(new ClientGetUserByEmailResponse(){Succeeded = false, Code = (int)ResultCodes.ServerError});
        }
    }

    /// <summary>
    /// Метод получения всех пользователей в системе админом
    /// </summary>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpGet(AuthApiRoute.GetAllUsers)]
    public async Task<IActionResult> GetAllUsersAsync()
    {
        try
        {
            var list_of_all_users = await _UserManager.Users.ToListAsync();
            return Ok(new ClientGetAllUsersResponse()
            {
                Succeeded = true, 
                Code      = (int)ResultCodes.Ok, 
                Users     = list_of_all_users
            });
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Произошла ошибка при получении пользователя {Ex}", ex);
            return Ok(new ClientGetAllUsersResponse(){Succeeded = false, Code = (int)ResultCodes.ServerError});
        }
    }

    /// <summary>
    /// Метод редактирования информации о пользователе
    /// </summary>
    /// <param name="EditUserRequest">Запрос админа</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpPut(AuthApiRoute.EditUserByEmail)]
    public async Task<IActionResult> EditUserNameByEmailAsync([FromBody] EditUserNameByEmailRequest? EditUserRequest)
    {
        try
        {
            var identity_user = await _UserManager.FindByEmailAsync(EditUserRequest.UserEmail);
            if (identity_user is not null)
            {
                identity_user.UserName = EditUserRequest.EditUserNickName;

                var identity_result = await _UserManager.UpdateAsync(identity_user);
                if (identity_result.Succeeded)
                {
                    var new_token = _authUtilits.CreateSessionToken(identity_user,
                        await _UserManager.GetRolesAsync(identity_user));
                    
                    return Ok(new ClientEditUserNameByEmailResponse()
                    {
                        Succeeded = true, 
                        Code      = (int) ResultCodes.Ok,
                        NewToken  = new_token,
                        Message   = $"Имя пользователя {identity_user.UserName} успешно изменена на {EditUserRequest.EditUserNickName}"
                    });
                }

                _Logger.Log(LogLevel.Information, "Не удалось обновить информацию пользователя");
                return Ok(new ClientEditUserNameByEmailResponse()
                {
                    Succeeded = false, 
                    Code      = (int)ResultCodes.NotFound,
                    Message   = "Не удалось изменить имя пользователя"
                });
            }

            _Logger.Log(LogLevel.Information,
                "Не удалось найти пользователя {Email}", EditUserRequest.UserEmail);
            return Ok(new ClientEditUserNameByEmailResponse()
            {
                Succeeded = false, 
                Code      = (int)ResultCodes.NotFound,
                Message   = $"Не удалось найти пользователя {EditUserRequest.UserEmail}"
            });
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Произошла ошибка при изменении имени пользователя {Ex}", ex);
            return Ok(new ClientEditUserNameByEmailResponse(){Succeeded = false, Code = (int)ResultCodes.ServerError});
        }
    }

    /// <summary>
    /// Метод удаления пользователя
    /// </summary>
    /// <param name="Email">Почта пользователя</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpDelete($"{AuthApiRoute.DeleteUserByEmail}"+"{Email}")]
    public async Task<IActionResult> DeleteUserByEmailAsync(string Email)
    {
        try
        {
            var identity_user = await _UserManager.FindByEmailAsync(Email);
            if (identity_user is not null)
            {
                var identity_result = await _UserManager.DeleteAsync(identity_user);
                if (identity_result.Succeeded)
                {
                    return Ok(new ClientDeleteUserByEmailResponse()
                    {
                        Succeeded = true, 
                        Code      = (int)ResultCodes.Ok, 
                        Message   = $"Пользователь {identity_user.Email} успешно удален"
                    });
                }

                _Logger.Log(LogLevel.Information, "Не удалось удалить пользователя {Email}", Email);
                return Ok(new ClientDeleteUserByEmailResponse()
                {
                    Succeeded = false, 
                    Code      = (int)ResultCodes.ApiError,
                    Message   = $"Не удалось удалить пользователя {Email}"
                });
            }

            _Logger.Log(LogLevel.Information, "Не удалось получить информацию о пользователе {Email}", Email);
            return Ok(new ClientDeleteUserByEmailResponse()
            {
                Succeeded = false, 
                Code      = (int)ResultCodes.NotFound,
                Message   = $"Не удалось получить информацию о пользователе {Email}"
            });
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Произошла ошибка при удалении пользователя {Ex}", ex);
            return Ok(new ClientDeleteUserByEmailResponse(){Succeeded = false, Code = (int)ResultCodes.ServerError});
        }
    }

    /// <summary>
    /// Метод удаления пользователей без регистрации (для очистки БД)
    /// </summary>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpDelete(AuthApiRoute.DeleteUserWithoutConfirm)]
    public async Task<IActionResult> DeleteUsersWithOutConfirmAsync()
    {
        try
        {
            var users_to_delete_list = await _UserManager
               .Users.Where(x => x.EmailConfirmed.Equals(false))
               .ToListAsync();
            foreach (var user in users_to_delete_list)
            {
                if (user.EmailConfirmed is false)
                {
                    await _UserManager.DeleteAsync(user);
                }
            }

            return Ok(new ClientDeleteUsersWithOutConfirmResponse()
            {
                Succeeded = true, 
                Code      = (int)ResultCodes.Ok, 
                Message   = "Пользователи без регистрации успешно удалены"
            });
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Произошла ошибка при удалении пользователей {Ex}", ex);
            return Ok(new ClientDeleteUsersWithOutConfirmResponse(){Succeeded = false, Code = (int)ResultCodes.ServerError});
        }
    }
}