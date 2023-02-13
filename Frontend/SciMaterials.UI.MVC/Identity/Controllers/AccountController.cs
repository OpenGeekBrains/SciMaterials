using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SciMaterials.Contracts;
using SciMaterials.DAL.AUTH.Contracts;
using SciMaterials.Contracts.Identity.API;
using SciMaterials.Contracts.Identity.API.Requests.Roles;
using SciMaterials.Contracts.Identity.API.Requests.Users;
using SciMaterials.Contracts.Identity.API.Responses.DTO;
using SciMaterials.Contracts.Identity.API.Responses.User;
using SciMaterials.Contracts.Result;

namespace SciMaterials.UI.MVC.Identity.Controllers;

/// <summary> Контроллер для регистрации и авторизации в системе аутентификации </summary>
[ApiController]
[Route(AuthApiRoute.AuthControllerName)]
public class AccountController : Controller, IIdentityApi
{
    private readonly UserManager<IdentityUser> _UserManager;
    private readonly SignInManager<IdentityUser> _SignInManager;
    private readonly RoleManager<IdentityRole> _RoleManager;
    private readonly IHttpContextAccessor _ContextAccessor;
    private readonly IAuthUtils _authUtilits;
    private readonly ILogger<AccountController> _Logger;

    public AccountController(
        UserManager<IdentityUser> UserManager,
        SignInManager<IdentityUser> SignInManager,
        RoleManager<IdentityRole> RoleManager,
        IHttpContextAccessor ContextAccessor,
        IAuthUtils authUtilits,
        ILogger<AccountController> Logger)
    {
        _UserManager = UserManager;
        _SignInManager = SignInManager;
        _RoleManager = RoleManager;
        _Logger = Logger;
        _ContextAccessor = ContextAccessor;
        _authUtilits = authUtilits;
    }
    
    [AllowAnonymous]
    [HttpPost(AuthApiRoute.Register)]
    public async Task<Result<RegisterUserResponse>> RegisterUserAsync([FromBody] RegisterRequest RegisterRequest, CancellationToken Cancel = default)
    {
        try
        {
            Cancel.ThrowIfCancellationRequested();

            var identity_user = new IdentityUser { Email = RegisterRequest.Email, UserName = RegisterRequest.NickName };
            var identity_result = await _UserManager.CreateAsync(identity_user, RegisterRequest.Password);

            Cancel.ThrowIfCancellationRequested();

            if (identity_result.Succeeded)
            {
                await _UserManager.AddToRoleAsync(identity_user, AuthApiRoles.User);
                var email_confirm_token = await _UserManager.GenerateEmailConfirmationTokenAsync(identity_user);

                var callback_url = Url.Action(
                    action: "ConfirmEmail",
                    controller: "Accounts",
                    values: new { UserId = identity_user.Id, ConfirmToken = email_confirm_token },
                    protocol: HttpContext.Request.Scheme);

                return Result<RegisterUserResponse>.Success(new RegisterUserResponse(callback_url));
            }

            var errors = identity_result.Errors.Select(e => e.Description).ToArray();
            _Logger.LogInformation(
                "Не удалось зарегистрировать пользователя {Email}: {errors}",
                RegisterRequest.Email,
                string.Join(",", errors));

            return Result<RegisterUserResponse>.Failure(Errors.Identity.Register.Fail);
        }
        catch (OperationCanceledException)
        {
            return Result<RegisterUserResponse>.Failure(Errors.App.OperationCanceled);
        }
        catch (Exception ex)
        {
            _Logger.LogError("Пользователя не удалось зарегистрировать {Ex}", ex);
            return Result<RegisterUserResponse>.Failure(Errors.Identity.Register.Unhandled);
        }
    }
    
    [AllowAnonymous]
    [HttpPost(AuthApiRoute.Login)]
    public async Task<Result<LoginUserResponse>> LoginUserAsync([FromBody] LoginRequest? LoginRequest, CancellationToken Cancel = default)
    {
        try
        {
            Cancel.ThrowIfCancellationRequested();

            var identity_user = await _UserManager.FindByEmailAsync(LoginRequest.Email);
            if (identity_user is null)
            {
                _Logger.LogWarning(
                    "Некорректно введены данные {Email}, {Password}",
                    LoginRequest.Email,
                    LoginRequest.Password);
                return Result<LoginUserResponse>.Failure(Errors.Identity.Login.UserNotFound);
            }

            Cancel.ThrowIfCancellationRequested();

            if (!await CheckIsEmailConfirmedAsync(identity_user))
            {
                _Logger.LogWarning("Email не подтверждён {Email}", LoginRequest.Email);
                return Result<LoginUserResponse>.Failure(Errors.Identity.Login.EmailNotConfirmed);
            }

            Cancel.ThrowIfCancellationRequested();

            var sign_in_result = await _SignInManager.PasswordSignInAsync(
            userName: identity_user.UserName,
            password: LoginRequest.Password,
            isPersistent: true,
            lockoutOnFailure: false);

            if (sign_in_result.Succeeded)
            {
                var identity_roles = await _UserManager.GetRolesAsync(identity_user);
                var session_token = _authUtilits.CreateSessionToken(identity_user, identity_roles);
                return Result<LoginUserResponse>.Success(new LoginUserResponse(session_token));
            }

            _Logger.LogWarning("Не удалось авторизовать пользователя {Email}", LoginRequest.Email);
            return Result<LoginUserResponse>.Failure(Errors.Identity.Login.Fail);
        }
        catch (OperationCanceledException)
        {
            return Result<LoginUserResponse>.Failure(Errors.App.OperationCanceled);
        }
        catch (Exception ex)
        {
            _Logger.LogError("Не удалось авторизовать пользователя {Ex}", ex);
            return Result<LoginUserResponse>.Failure(Errors.Identity.Login.Unhandled);
        }
    }
    
    [HttpPost(AuthApiRoute.Logout)]
    public async Task<Result> LogoutUserAsync(CancellationToken Cancel = default)
    {
        try
        {
            Cancel.ThrowIfCancellationRequested();

            await _SignInManager.SignOutAsync();
            return Result.Success();
        }
        catch (OperationCanceledException)
        {
            return Result.Failure(Errors.App.OperationCanceled);
        }
        catch (Exception ex)
        {
            _Logger.LogError("Не удалось выйти из системы {Ex}", ex);
            return Result.Failure(Errors.Identity.Logout.Unhandled);
        }
    }
    
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpPost(AuthApiRoute.ChangePassword)]
    public async Task<Result> ChangePasswordAsync([FromBody] ChangePasswordRequest ChangePasswordRequest, CancellationToken Cancel = default)
    {
        try
        {
            var current_user_name = _ContextAccessor.HttpContext?.User.Identity?.Name;
            if (current_user_name is not { Length: > 0 })
            {
                _Logger.LogWarning("Change password request called without authorization data");
                return Result.Failure(Errors.Identity.ChangePassword.MissAuthorizationData);
            }

            Cancel.ThrowIfCancellationRequested();

            var identity_user = await _UserManager.FindByNameAsync(current_user_name);
            if (identity_user is null)
            {
                _Logger.LogWarning("Некорректно введены данные {Email}", current_user_name);
                return Result.Failure(Errors.Identity.ChangePassword.NotFound);
            }

            Cancel.ThrowIfCancellationRequested();

            if (!await CheckIsEmailConfirmedAsync(identity_user))
            {
                _Logger.LogWarning("Email не подтверждён {Email}", identity_user.Email);
                return Result.Failure(Errors.Identity.ChangePassword.EmailNotConfirmed);
            }

            Cancel.ThrowIfCancellationRequested();

            var identity_result = await _UserManager.ChangePasswordAsync(
                identity_user,
                ChangePasswordRequest.CurrentPassword,
                ChangePasswordRequest.NewPassword);

            if (identity_result.Succeeded)
            {
                await _SignInManager.RefreshSignInAsync(identity_user);

                return Result.Success();
            }

            _Logger.LogWarning(
                "Не удалось изменить пароль {CurrentPassword}, {NewPassword}",
                ChangePasswordRequest.CurrentPassword,
                ChangePasswordRequest.NewPassword);
            return Result.Failure(Errors.Identity.ChangePassword.Fail);
        }
        catch (OperationCanceledException)
        {
            return Result.Failure(Errors.App.OperationCanceled);
        }
        catch (Exception ex)
        {
            _Logger.LogError("Произошла ошибка при смене пароля {Ex}", ex);
            return Result.Failure(Errors.Identity.ChangePassword.Unhandled);
        }
    }
    
    [HttpGet(AuthApiRoute.RefreshToken)]
    public async Task<Result<RefreshTokenResponse>> GetRefreshTokenAsync(CancellationToken Cancel = default)
    {
        try
        {
            Cancel.ThrowIfCancellationRequested();

            // TODO: Не обращай внимание, я тут буду править.
            var headersAuthorization = (string?)_ContextAccessor.HttpContext?.Request.Headers.Authorization;
            // TODO: validation
            var jwtToken = headersAuthorization.Remove(0, 7);

            var token = new JwtSecurityTokenHandler().ReadJwtToken(jwtToken);

            var userEmail = (string)token.Payload.First(x => x.Key.Equals("email")).Value;
            // TODO: validation

            Cancel.ThrowIfCancellationRequested();

            var identity_user = await _UserManager.FindByEmailAsync(userEmail);
            // TODO: validation

            Cancel.ThrowIfCancellationRequested();

            var identity_roles = await _UserManager.GetRolesAsync(identity_user);
            var new_session_token = _authUtilits.CreateSessionToken(identity_user, identity_roles);

            if (!string.IsNullOrEmpty(new_session_token))
            {
                return Result<RefreshTokenResponse>.Success(new RefreshTokenResponse(new_session_token));
            }

            _Logger.LogWarning("Не удалось обновить токен пользователя");
            return Result<RefreshTokenResponse>.Failure(Errors.Identity.GetRefreshToken.Fail);
        }
        catch (OperationCanceledException)
        {
            return Result<RefreshTokenResponse>.Failure(Errors.App.OperationCanceled);
        }
        catch (Exception ex)
        {
            _Logger.LogError("Не удалось обновить токен пользователя {Ex}", ex);
            return Result<RefreshTokenResponse>.Failure(Errors.Identity.GetRefreshToken.Unhandled);
        }
    }

    
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpPost(AuthApiRoute.CreateRole)]
    public async Task<Result> CreateRoleAsync([FromBody] CreateRoleRequest CreateRoleRequest, CancellationToken Cancel = default)
    {
        try
        {
            Cancel.ThrowIfCancellationRequested();

            var identity_result = await _RoleManager.CreateAsync(new IdentityRole(CreateRoleRequest.RoleName.ToLower()));
            if (identity_result.Succeeded)
            {
                return Result.Success();
            }

            _Logger.LogWarning("Не удалось создать роль");
            return Result.Failure(Errors.Identity.CreateRole.Fail);
        }
        catch (OperationCanceledException)
        {
            return Result.Failure(Errors.App.OperationCanceled);
        }
        catch (Exception ex)
        {
            _Logger.LogError("Произошла ошибка при создании роли {Ex}", ex);
            return Result.Failure(Errors.Identity.CreateRole.Unhandled);
        }
    }
    
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpGet(AuthApiRoute.GetAllRoles)]
    public async Task<Result<IEnumerable<AuthRole>>> GetAllRolesAsync(CancellationToken Cancel = default)
    {
        try
        {
            var identity_roles_list = await _RoleManager.Roles.ToListAsync(cancellationToken: Cancel);
            var roles = identity_roles_list
               .Select(role => new AuthRole()
               {
                   Id = role.Id,
                   RoleName = role.Name,
               })
               .ToArray();

            return Result<IEnumerable<AuthRole>>.Success(roles);
        }
        catch (OperationCanceledException)
        {
            return Result<IEnumerable<AuthRole>>.Failure(Errors.App.OperationCanceled);
        }
        catch (Exception ex)
        {
            _Logger.LogError("Произошла ошибка при запросе ролей {Ex}", ex);
            return Result<IEnumerable<AuthRole>>.Failure(Errors.Identity.GetAllRoles.Unhandled);
        }
    }
    
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpGet($"{AuthApiRoute.GetRoleById}/" + "{RoleId}")]
    public async Task<Result<AuthRole>> GetRoleByIdAsync([FromRoute] string RoleId, CancellationToken Cancel = default)
    {
        try
        {
            Cancel.ThrowIfCancellationRequested();

            var identity_role = await _RoleManager.FindByIdAsync(RoleId);
            if (identity_role is null)
            {
                _Logger.LogWarning("Не удалось получить роль");
                return Result<AuthRole>.Failure(Errors.Identity.GetRoleById.NotFound);
            }

            var role = new AuthRole { Id = identity_role.Id, RoleName = identity_role.Name };
            return Result<AuthRole>.Success(role);
        }
        catch (OperationCanceledException)
        {
            return Result<AuthRole>.Failure(Errors.App.OperationCanceled);
        }
        catch (Exception ex)
        {
            _Logger.LogError("Произошла ошибка при запросе роли {Ex}", ex);
            return Result<AuthRole>.Failure(Errors.Identity.GetRoleById.Unhandled);
        }
    }
    
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpPut(AuthApiRoute.EditRoleNameById)]
    public async Task<Result> EditRoleNameByIdAsync([FromBody] EditRoleNameByIdRequest EditRoleRequest, CancellationToken Cancel = default)
    {
        try
        {
            Cancel.ThrowIfCancellationRequested();

            var identity_role = await _RoleManager.FindByIdAsync(EditRoleRequest.RoleId);
            if (identity_role is null)
            {
                _Logger.LogWarning("Не удалось найти роль {RoleId}", EditRoleRequest.RoleId);
                return Result.Failure(Errors.Identity.EditRoleNameById.NotFound);
            }

            identity_role.Name = EditRoleRequest.RoleName.ToLower();
            identity_role.NormalizedName = EditRoleRequest.RoleName.ToUpper();

            Cancel.ThrowIfCancellationRequested();

            var identity_result = await _RoleManager.UpdateAsync(identity_role);
            if (identity_result.Succeeded)
            {
                return Result.Success();
            }

            _Logger.LogWarning("Не удалось изменить роль");
            return Result.Failure(Errors.Identity.EditRoleNameById.Fail);
        }
        catch (OperationCanceledException)
        {
            return Result.Failure(Errors.App.OperationCanceled);
        }
        catch (Exception ex)
        {
            _Logger.LogError("Произошла ошибка при редактировании роли {Ex}", ex);
            return Result.Failure(Errors.Identity.EditRoleNameById.Unhandled);
        }
    }
    
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpDelete($"{AuthApiRoute.DeleteRoleById}/" + "{RoleId}")]
    public async Task<Result> DeleteRoleByIdAsync([FromRoute] string RoleId, CancellationToken Cancel = default)
    {
        try
        {
            Cancel.ThrowIfCancellationRequested();

            var identity_role = await _RoleManager.FindByIdAsync(RoleId);
            if (identity_role is null)
            {
                _Logger.LogWarning("Не удалось найти роль");
                return Result.Failure(Errors.Identity.DeleteRoleById.NotFound);
            }

            Cancel.ThrowIfCancellationRequested();

            // check that user not try to delete ADMIN or USER roles
            if (!_authUtilits.CheckToDeleteAdminOrUserRoles(identity_role))
            {
                _Logger.LogWarning("Не удалось найти роль");
                return Result.Failure(Errors.Identity.DeleteRoleById.TryToDeleteSystemRoles);
            }

            Cancel.ThrowIfCancellationRequested();

            var identity_result = await _RoleManager.DeleteAsync(identity_role);
            if (identity_result.Succeeded)
            {
                return Result.Success();
            }

            _Logger.LogWarning("Не удалось удалить роль");
            return Result.Failure(Errors.Identity.DeleteRoleById.Fail);
        }
        catch (OperationCanceledException)
        {
            return Result.Failure(Errors.App.OperationCanceled);
        }
        catch (Exception ex)
        {
            _Logger.LogError("Произошла ошибка при удалении роли {Ex}", ex);
            return Result.Failure(Errors.Identity.DeleteRoleById.Unhandled);
        }
    }
    
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpPost(AuthApiRoute.AddRoleToUser)]
    public async Task<Result> AddRoleToUserAsync([FromBody] AddRoleToUserRequest AddRoleToUserRequest, CancellationToken Cancel = default)
    {
        try
        {
            var normalized_role_name = AddRoleToUserRequest.RoleName.ToLowerInvariant();

            if (await _RoleManager.Roles.AnyAsync(x => x.Name == normalized_role_name, cancellationToken: Cancel))
            {
                _Logger.LogWarning("Роль не зарегистрированна {Role}", AddRoleToUserRequest.RoleName);
                return Result.Failure(Errors.Identity.AddRoleToUser.RoleNotFound);
            }

            var identity_user = await _UserManager.FindByEmailAsync(AddRoleToUserRequest.Email);
            if (identity_user is null)
            {
                _Logger.LogWarning("Пользователь не найден {User}", AddRoleToUserRequest.Email);
                return Result.Failure(Errors.Identity.AddRoleToUser.UserNotFound);
            }

            Cancel.ThrowIfCancellationRequested();

            if (await _UserManager.IsInRoleAsync(identity_user, normalized_role_name))
            {
                _Logger.LogWarning("Пользователь {User} уже имеет данную роль {Role}", AddRoleToUserRequest.Email, AddRoleToUserRequest.RoleName);
                return Result.Failure(Errors.Identity.AddRoleToUser.UserAlreadyInRole);
            }

            Cancel.ThrowIfCancellationRequested();

            var role_added_result = await _UserManager.AddToRoleAsync(identity_user, normalized_role_name);
            if (role_added_result.Succeeded)
            {
                // TODO: Schedule system to user update token when he will be signed in
                return Result.Success();
            }

            _Logger.LogWarning("Произошла ошибка при присвоении роли пользователю");
            return Result.Failure(Errors.Identity.AddRoleToUser.Fail);
        }
        catch (OperationCanceledException)
        {
            return Result.Failure(Errors.App.OperationCanceled);
        }
        catch (Exception ex)
        {
            _Logger.LogError("Произошла ошибка при присвоении роли пользователю {Ex}", ex);
            return Result.Failure(Errors.Identity.AddRoleToUser.Unhandled);
        }
    }
    
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpDelete($"{AuthApiRoute.DeleteUserRoleByEmail}/" + "{Email}/{RoleName}")]
    public async Task<Result> DeleteUserRoleByEmailAsync([FromRoute] string Email, [FromRoute] string RoleName, CancellationToken Cancel = default)
    {
        try
        {
            var normalized_role_name = RoleName.ToLowerInvariant();
            if (await _RoleManager.Roles.AnyAsync(x => x.Name == normalized_role_name, cancellationToken: Cancel))
            {
                _Logger.LogWarning("Роль не зарегистрированна {Role}", RoleName);
                return Result.Failure(Errors.Identity.RemoveRoleFromUserByEmail.RoleNotFound);
            }

            var identity_user = await _UserManager.FindByEmailAsync(Email);
            if (identity_user is null)
            {
                _Logger.LogWarning("Пользователь не найден {User}", Email);
                return Result.Failure(Errors.Identity.RemoveRoleFromUserByEmail.UserNotFound);
            }

            Cancel.ThrowIfCancellationRequested();

            if (!await _UserManager.IsInRoleAsync(identity_user, normalized_role_name))
            {
                _Logger.LogWarning("Пользователь {User} не имеет данную роль {Role}", Email, RoleName);
                return Result.Failure(Errors.Identity.RemoveRoleFromUserByEmail.UserNotInRole);
            }

            Cancel.ThrowIfCancellationRequested();

            // check that user not trying to remove super admin from admin role
            if (!_authUtilits.CheckToDeleteSAInRoleAdmin(identity_user, RoleName.ToLower()))
            {
                _Logger.LogWarning("Попытка понизить супер админа в должности");
                return Result.Failure(Errors.Identity.RemoveRoleFromUserByEmail.TryToDownSuperAdmin);
            }

            Cancel.ThrowIfCancellationRequested();

            var role_removed_result = await _UserManager.RemoveFromRoleAsync(identity_user, RoleName.ToLower());
            if (role_removed_result.Succeeded)
            {
                // TODO: Schedule system to user update token when he will be signed in
                return Result.Success();
            }

            _Logger.LogWarning("Некорректно введены данные {Email}, {RoleName}", Email, RoleName);
            return Result.Failure(Errors.Identity.RemoveRoleFromUserByEmail.Fail);
        }
        catch (OperationCanceledException)
        {
            return Result.Failure(Errors.App.OperationCanceled);
        }
        catch (Exception ex)
        {
            _Logger.LogError("Произошла ошибка при удалении роли пользователю {Ex}", ex);
            return Result.Failure(Errors.Identity.RemoveRoleFromUserByEmail.Unhandled);
        }
    }
    
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpGet($"{AuthApiRoute.GetAllUserRolesByEmail}/" + "{Email}")]
    public async Task<Result<IEnumerable<AuthRole>>> GetUserRolesAsync([FromRoute] string Email, CancellationToken Cancel = default)
    {
        try
        {
            Cancel.ThrowIfCancellationRequested();

            var identity_user = await _UserManager.FindByEmailAsync(Email);
            if (identity_user is null)
            {
                _Logger.LogWarning("Пользователь не найден {User}", Email);
                return Result<IEnumerable<AuthRole>>.Failure(Errors.Identity.GetUserRoles.UserNotFound);
            }
            
            var roles = await GetUserRolesAsync(identity_user, Cancel);

            return Result<IEnumerable<AuthRole>>.Success(roles);
        }
        catch (OperationCanceledException)
        {
            return Result<IEnumerable<AuthRole>>.Failure(Errors.App.OperationCanceled);
        }
        catch (Exception ex)
        {
            _Logger.LogError("Произошла ошибка при получении списка ролей пользователя {Ex}", ex);
            return Result<IEnumerable<AuthRole>>.Failure(Errors.Identity.GetUserRoles.Unhandled);
        }
    }
    
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpGet($"{AuthApiRoute.GetUserByEmail}/" + "{Email}")]
    public async Task<Result<AuthUser>> GetUserByEmailAsync([FromRoute] string Email, CancellationToken Cancel = default)
    {
        try
        {
            Cancel.ThrowIfCancellationRequested();

            var identity_user = await _UserManager.FindByEmailAsync(Email);
            if (identity_user is null)
            {
                _Logger.LogWarning("Не удалось получить информации о пользователе");
                return Result<AuthUser>.Failure(Errors.Identity.GetUserByEmail.NotFound);
            }

            var roles = await GetUserRolesAsync(identity_user, Cancel);

            AuthUser user = new()
            {
                Id = identity_user.Id,
                Email = Email,
                UserName = identity_user.UserName,
                UserRoles = roles,
            };
            return Result<AuthUser>.Success(user);
        }
        catch (OperationCanceledException)
        {
            return Result<AuthUser>.Failure(Errors.App.OperationCanceled);
        }
        catch (Exception ex)
        {
            _Logger.LogError("Произошла ошибка при получении пользователей {Ex}", ex);
            return Result<AuthUser>.Failure(Errors.Identity.GetUserByEmail.Unhandled);
        }
    }
    
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpGet(AuthApiRoute.GetAllUsers)]
    public async Task<Result<IEnumerable<AuthUser>>> GetAllUsersAsync(CancellationToken Cancel = default)
    {
        try
        {
            var list_of_all_users = await _UserManager.Users.ToListAsync(cancellationToken: Cancel);
            var users             = new List<AuthUser>();
            foreach (var user in list_of_all_users)
            {
                var roles = await GetUserRolesAsync(user, Cancel);
                users.Add(new AuthUser
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    UserRoles = roles
                });
            }

            return Result<IEnumerable<AuthUser>>.Success(users);
        }
        catch (OperationCanceledException)
        {
            return Result<IEnumerable<AuthUser>>.Failure(Errors.App.OperationCanceled);
        }
        catch (Exception ex)
        {
            _Logger.LogError("Произошла ошибка при получении списка пользователей {Ex}", ex);
            return Result<IEnumerable<AuthUser>>.Failure(Errors.Identity.GetAllUsers.Unhandled);
        }
    }
    
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpPut(AuthApiRoute.EditUserByEmail)]
    public async Task<Result<EditUserNameResponse>> EditUserNameByEmailAsync([FromBody] EditUserNameByEmailRequest EditUserRequest, CancellationToken Cancel = default)
    {
        try
        {
            Cancel.ThrowIfCancellationRequested();

            var identity_user = await _UserManager.FindByEmailAsync(EditUserRequest.UserEmail);
            if (identity_user is null)
            {
                _Logger.LogWarning("Не удалось найти пользователя {Email}", EditUserRequest.UserEmail);
                return Result<EditUserNameResponse>.Failure(Errors.Identity.EditUserName.NotFound);
            }

            identity_user.UserName = EditUserRequest.EditUserNickName;

            Cancel.ThrowIfCancellationRequested();

            var identity_result = await _UserManager.UpdateAsync(identity_user);
            if (identity_result.Succeeded)
            {
                var new_token = _authUtilits.CreateSessionToken(
                    identity_user,
                    await _UserManager.GetRolesAsync(identity_user));

                return Result<EditUserNameResponse>.Success(new EditUserNameResponse(new_token));
            }

            _Logger.LogWarning("Не удалось обновить информацию пользователя");
            return Result<EditUserNameResponse>.Failure(Errors.Identity.EditUserName.Fail);
        }
        catch (OperationCanceledException)
        {
            return Result<EditUserNameResponse>.Failure(Errors.App.OperationCanceled);
        }
        catch (Exception ex)
        {
            _Logger.LogError("Произошла ошибка при изменении имени пользователя {Ex}", ex);
            return Result<EditUserNameResponse>.Failure(Errors.Identity.EditUserName.Unhandled);
        }
    }
    
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpDelete($"{AuthApiRoute.DeleteUserByEmail}/" + "{Email}")]
    public async Task<Result> DeleteUserByEmailAsync([FromRoute] string Email, CancellationToken Cancel = default)
    {
        try
        {
            Cancel.ThrowIfCancellationRequested();

            var identity_user = await _UserManager.FindByEmailAsync(Email.ToLower());
            if (identity_user is null)
            {
                _Logger.LogWarning("Не удалось получить информацию о пользователе {Email}", Email);
                return Result.Failure(Errors.Identity.DeleteUser.NotFound);
            }

            Cancel.ThrowIfCancellationRequested();

            if (_authUtilits.CheckToDeleteSA(identity_user))
            {
                var identity_result = await _UserManager.DeleteAsync(identity_user);
                if (identity_result.Succeeded)
                {
                    return Result.Success();
                }
            }

            _Logger.LogWarning("Не удалось удалить пользователя {Email}", Email);
            return Result.Failure(Errors.Identity.DeleteUser.Fail);
        }
        catch (OperationCanceledException)
        {
            return Result.Failure(Errors.App.OperationCanceled);
        }
        catch (Exception ex)
        {
            _Logger.LogError("Произошла ошибка при удалении пользователя {Ex}", ex);
            return Result.Failure(Errors.Identity.DeleteUser.Unhandled);
        }
    }

    private async Task<List<AuthRole>> GetUserRolesAsync(IdentityUser IdentityUser, CancellationToken Cancel = default)
    {
        try
        {
            Cancel.ThrowIfCancellationRequested();

            var user_roles = (await _UserManager.GetRolesAsync(IdentityUser)).ToHashSet();
            var roles = await _RoleManager.Roles
               .Where(x => user_roles.Contains(x.Name))
               .Select(x => new AuthRole()
                {
                    Id       = x.Id,
                    RoleName = x.Name,
                })
               .ToListAsync(cancellationToken: Cancel);
            return roles;
        }
        catch (Exception ex)
        {
            _Logger.LogError("Произошла ошибка при получении списка ролей пользователей {Ex}", ex);
            throw;
        }
    }

    private Task<bool> CheckIsEmailConfirmedAsync(IdentityUser identityUser) => _UserManager.IsEmailConfirmedAsync(identityUser);
}