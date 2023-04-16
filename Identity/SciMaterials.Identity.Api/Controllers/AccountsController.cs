using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SciMaterials.Contracts;
using SciMaterials.Contracts.Identity.API;
using SciMaterials.Contracts.Identity.API.Requests.Roles;
using SciMaterials.Contracts.Identity.API.Requests.Users;
using SciMaterials.Contracts.Identity.API.Responses.DTO;
using SciMaterials.Contracts.Identity.API.Responses.User;
using SciMaterials.Contracts.Result;
using SciMaterials.DAL.AUTH.Contracts;

namespace SciMaterials.Identity.Api.Controllers;

/// <summary> Контроллер для регистрации и авторизации в системе аутентификации </summary>
[ApiController]
[Route(AuthApiRoute.AuthControllerName)]
public class AccountsController : Controller, IIdentityApi
{
    private readonly UserManager<IdentityUser> _UserManager;
    private readonly SignInManager<IdentityUser> _SignInManager;
    private readonly RoleManager<IdentityRole> _RoleManager;
    private readonly IHttpContextAccessor _ContextAccessor;
    private readonly IAuthUtils _authUtilits;
    private readonly ILogger<AccountsController> _Logger;

    public AccountsController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        IHttpContextAccessor contextAccessor,
        IAuthUtils authUtilits,
        ILogger<AccountsController> logger)
    {
        _UserManager = userManager;
        _SignInManager = signInManager;
        _RoleManager = roleManager;
        _Logger = logger;
        _ContextAccessor = contextAccessor;
        _authUtilits = authUtilits;
    }
    
    [AllowAnonymous]
    [HttpPost(AuthApiRoute.Register)]
    public async Task<Result<RegisterUserResponse>> RegisterUserAsync([FromBody] RegisterRequest registerRequest, CancellationToken cancel = default)
    {
        try
        {
            cancel.ThrowIfCancellationRequested();

            var identityUser = new IdentityUser { Email = registerRequest.Email, UserName = registerRequest.NickName };
            var identityResult = await _UserManager.CreateAsync(identityUser, registerRequest.Password);

            cancel.ThrowIfCancellationRequested();

            if (identityResult.Succeeded)
            {
                await _UserManager.AddToRoleAsync(identityUser, AuthApiRoles.User);
                var emailConfirmToken = await _UserManager.GenerateEmailConfirmationTokenAsync(identityUser);

                var callbackUrl = $"http://localhost:5002/Accounts/ConfirmEmail?UserId={identityUser.Id}&ConfirmToken={emailConfirmToken}";

                return Result<RegisterUserResponse>.Success(new RegisterUserResponse(callbackUrl));
            }

            var errors = identityResult.Errors.Select(e => e.Description).ToArray();
            _Logger.LogInformation(
                "Не удалось зарегистрировать пользователя {Email}: {errors}",
                registerRequest.Email,
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
    public async Task<Result<LoginUserResponse>> LoginUserAsync([FromBody] LoginRequest loginRequest, CancellationToken cancel = default)
    {
        try
        {
            cancel.ThrowIfCancellationRequested();

            var identityUser = await _UserManager.FindByEmailAsync(loginRequest.Email);
            if (identityUser is null)
            {
                _Logger.LogWarning(
                    "Некорректно введены данные {Email}, {Password}",
                    loginRequest.Email,
                    loginRequest.Password);
                return Result<LoginUserResponse>.Failure(Errors.Identity.Login.UserNotFound);
            }

            cancel.ThrowIfCancellationRequested();

            if (!await CheckIsEmailConfirmedAsync(identityUser))
            {
                _Logger.LogWarning("Email не подтверждён {Email}", loginRequest.Email);
                return Result<LoginUserResponse>.Failure(Errors.Identity.Login.EmailNotConfirmed);
            }

            cancel.ThrowIfCancellationRequested();

            var signInResult = await _SignInManager.PasswordSignInAsync(
            userName: identityUser.UserName,
            password: loginRequest.Password,
            isPersistent: true,
            lockoutOnFailure: false);

            if (signInResult.Succeeded)
            {
                var identityRoles = await _UserManager.GetRolesAsync(identityUser);
                var sessionToken = _authUtilits.CreateSessionToken(identityUser, identityRoles);
                return Result<LoginUserResponse>.Success(new LoginUserResponse(sessionToken));
            }

            _Logger.LogWarning("Не удалось авторизовать пользователя {Email}", loginRequest.Email);
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
    public async Task<Result> LogoutUserAsync(CancellationToken cancel = default)
    {
        try
        {
            cancel.ThrowIfCancellationRequested();

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
    public async Task<Result> ChangePasswordAsync([FromBody] ChangePasswordRequest changePasswordRequest, CancellationToken cancel = default)
    {
        try
        {
            var currentUserName = _ContextAccessor.HttpContext?.User.Identity?.Name;
            if (currentUserName is not { Length: > 0 })
            {
                _Logger.LogWarning("Change password request called without authorization data");
                return Result.Failure(Errors.Identity.ChangePassword.MissAuthorizationData);
            }

            cancel.ThrowIfCancellationRequested();

            var identityUser = await _UserManager.FindByNameAsync(currentUserName);
            if (identityUser is null)
            {
                _Logger.LogWarning("Некорректно введены данные {Email}", currentUserName);
                return Result.Failure(Errors.Identity.ChangePassword.NotFound);
            }

            cancel.ThrowIfCancellationRequested();

            if (!await CheckIsEmailConfirmedAsync(identityUser))
            {
                _Logger.LogWarning("Email не подтверждён {Email}", identityUser.Email);
                return Result.Failure(Errors.Identity.ChangePassword.EmailNotConfirmed);
            }

            cancel.ThrowIfCancellationRequested();

            var identityResult = await _UserManager.ChangePasswordAsync(
                identityUser,
                changePasswordRequest.CurrentPassword,
                changePasswordRequest.NewPassword);

            if (identityResult.Succeeded)
            {
                await _SignInManager.RefreshSignInAsync(identityUser);

                return Result.Success();
            }

            _Logger.LogWarning(
                "Не удалось изменить пароль {CurrentPassword}, {NewPassword}",
                changePasswordRequest.CurrentPassword,
                changePasswordRequest.NewPassword);
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
    public async Task<Result<RefreshTokenResponse>> GetRefreshTokenAsync(CancellationToken cancel = default)
    {
        try
        {
            cancel.ThrowIfCancellationRequested();

            // TODO: Не обращай внимание, я тут буду править.
            var headersAuthorization = (string?)_ContextAccessor.HttpContext?.Request.Headers.Authorization;
            // TODO: validation
            var jwtToken = headersAuthorization.Remove(0, 7);

            var token = new JwtSecurityTokenHandler().ReadJwtToken(jwtToken);

            var userEmail = (string)token.Payload.First(x => x.Key.Equals("email")).Value;
            // TODO: validation

            cancel.ThrowIfCancellationRequested();

            var identityUser = await _UserManager.FindByEmailAsync(userEmail);
            // TODO: validation

            cancel.ThrowIfCancellationRequested();

            var identityRoles = await _UserManager.GetRolesAsync(identityUser);
            var newSessionToken = _authUtilits.CreateSessionToken(identityUser, identityRoles);

            if (!string.IsNullOrEmpty(newSessionToken))
            {
                return Result<RefreshTokenResponse>.Success(new RefreshTokenResponse(newSessionToken));
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
    public async Task<Result> CreateRoleAsync([FromBody] CreateRoleRequest createRoleRequest, CancellationToken cancel = default)
    {
        try
        {
            cancel.ThrowIfCancellationRequested();

            var identityResult = await _RoleManager.CreateAsync(new IdentityRole(createRoleRequest.RoleName.ToLower()));
            if (identityResult.Succeeded)
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
    public async Task<Result<IEnumerable<AuthRole>>> GetAllRolesAsync(CancellationToken cancel = default)
    {
        try
        {
            var identityRolesList = await _RoleManager.Roles.ToListAsync(cancellationToken: cancel);
            var roles = identityRolesList
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
    [HttpGet($"{AuthApiRoute.GetRoleById}/{{roleId}}")]
    public async Task<Result<AuthRole>> GetRoleByIdAsync([FromRoute] string roleId, CancellationToken cancel = default)
    {
        try
        {
            cancel.ThrowIfCancellationRequested();

            var identityRole = await _RoleManager.FindByIdAsync(roleId);
            if (identityRole is null)
            {
                _Logger.LogWarning("Не удалось получить роль");
                return Result<AuthRole>.Failure(Errors.Identity.GetRoleById.NotFound);
            }

            var role = new AuthRole { Id = identityRole.Id, RoleName = identityRole.Name };
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
    public async Task<Result> EditRoleNameByIdAsync([FromBody] EditRoleNameByIdRequest editRoleRequest, CancellationToken cancel = default)
    {
        try
        {
            cancel.ThrowIfCancellationRequested();

            var identityRole = await _RoleManager.FindByIdAsync(editRoleRequest.RoleId);
            if (identityRole is null)
            {
                _Logger.LogWarning("Не удалось найти роль {RoleId}", editRoleRequest.RoleId);
                return Result.Failure(Errors.Identity.EditRoleNameById.NotFound);
            }

            identityRole.Name = editRoleRequest.RoleName.ToLower();
            identityRole.NormalizedName = editRoleRequest.RoleName.ToUpper();

            cancel.ThrowIfCancellationRequested();

            var identityResult = await _RoleManager.UpdateAsync(identityRole);
            if (identityResult.Succeeded)
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
    [HttpDelete($"{AuthApiRoute.DeleteRoleById}/{{roleId}}")]
    public async Task<Result> DeleteRoleByIdAsync([FromRoute] string roleId, CancellationToken cancel = default)
    {
        try
        {
            cancel.ThrowIfCancellationRequested();

            var identityRole = await _RoleManager.FindByIdAsync(roleId);
            if (identityRole is null)
            {
                _Logger.LogWarning("Не удалось найти роль");
                return Result.Failure(Errors.Identity.DeleteRoleById.NotFound);
            }

            cancel.ThrowIfCancellationRequested();

            // check that user not try to delete ADMIN or USER roles
            if (!_authUtilits.CheckToDeleteAdminOrUserRoles(identityRole))
            {
                _Logger.LogWarning("Не удалось найти роль");
                return Result.Failure(Errors.Identity.DeleteRoleById.TryToDeleteSystemRoles);
            }

            cancel.ThrowIfCancellationRequested();

            var identityResult = await _RoleManager.DeleteAsync(identityRole);
            if (identityResult.Succeeded)
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
    public async Task<Result> AddRoleToUserAsync([FromBody] AddRoleToUserRequest addRoleToUserRequest, CancellationToken cancel = default)
    {
        try
        {
            var normalizedRoleName = addRoleToUserRequest.RoleName.ToLowerInvariant();

            if (await _RoleManager.Roles.AnyAsync(x => x.Name == normalizedRoleName, cancellationToken: cancel))
            {
                _Logger.LogWarning("Роль не зарегистрированна {Role}", addRoleToUserRequest.RoleName);
                return Result.Failure(Errors.Identity.AddRoleToUser.RoleNotFound);
            }

            var identityUser = await _UserManager.FindByEmailAsync(addRoleToUserRequest.Email);
            if (identityUser is null)
            {
                _Logger.LogWarning("Пользователь не найден {User}", addRoleToUserRequest.Email);
                return Result.Failure(Errors.Identity.AddRoleToUser.UserNotFound);
            }

            cancel.ThrowIfCancellationRequested();

            if (await _UserManager.IsInRoleAsync(identityUser, normalizedRoleName))
            {
                _Logger.LogWarning("Пользователь {User} уже имеет данную роль {Role}", addRoleToUserRequest.Email, addRoleToUserRequest.RoleName);
                return Result.Failure(Errors.Identity.AddRoleToUser.UserAlreadyInRole);
            }

            cancel.ThrowIfCancellationRequested();

            var roleAddedResult = await _UserManager.AddToRoleAsync(identityUser, normalizedRoleName);
            if (roleAddedResult.Succeeded)
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
    [HttpDelete($"{AuthApiRoute.DeleteUserRoleByEmail}/{{email}}/{{RoleName}}")]
    public async Task<Result> DeleteUserRoleByEmailAsync([FromRoute] string email, [FromRoute] string roleName, CancellationToken cancel = default)
    {
        try
        {
            var normalizedRoleName = roleName.ToLowerInvariant();
            if (await _RoleManager.Roles.AnyAsync(x => x.Name == normalizedRoleName, cancellationToken: cancel))
            {
                _Logger.LogWarning("Роль не зарегистрированна {Role}", roleName);
                return Result.Failure(Errors.Identity.RemoveRoleFromUserByEmail.RoleNotFound);
            }

            var identityUser = await _UserManager.FindByEmailAsync(email);
            if (identityUser is null)
            {
                _Logger.LogWarning("Пользователь не найден {User}", email);
                return Result.Failure(Errors.Identity.RemoveRoleFromUserByEmail.UserNotFound);
            }

            cancel.ThrowIfCancellationRequested();

            if (!await _UserManager.IsInRoleAsync(identityUser, normalizedRoleName))
            {
                _Logger.LogWarning("Пользователь {User} не имеет данную роль {Role}", email, roleName);
                return Result.Failure(Errors.Identity.RemoveRoleFromUserByEmail.UserNotInRole);
            }

            cancel.ThrowIfCancellationRequested();

            // check that user not trying to remove super admin from admin role
            if (!_authUtilits.CheckToDeleteSAInRoleAdmin(identityUser, roleName.ToLower()))
            {
                _Logger.LogWarning("Попытка понизить супер админа в должности");
                return Result.Failure(Errors.Identity.RemoveRoleFromUserByEmail.TryToDownSuperAdmin);
            }

            cancel.ThrowIfCancellationRequested();

            var roleRemovedResult = await _UserManager.RemoveFromRoleAsync(identityUser, roleName.ToLower());
            if (roleRemovedResult.Succeeded)
            {
                // TODO: Schedule system to user update token when he will be signed in
                return Result.Success();
            }

            _Logger.LogWarning("Некорректно введены данные {Email}, {RoleName}", email, roleName);
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
    [HttpGet($"{AuthApiRoute.GetAllUserRolesByEmail}/{{email}}")]
    public async Task<Result<IEnumerable<AuthRole>>> GetUserRolesAsync([FromRoute] string email, CancellationToken cancel = default)
    {
        try
        {
            cancel.ThrowIfCancellationRequested();

            var identityUser = await _UserManager.FindByEmailAsync(email);
            if (identityUser is null)
            {
                _Logger.LogWarning("Пользователь не найден {User}", email);
                return Result<IEnumerable<AuthRole>>.Failure(Errors.Identity.GetUserRoles.UserNotFound);
            }
            
            var roles = await GetUserRolesAsync(identityUser, cancel);

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
    [HttpGet($"{AuthApiRoute.GetUserByEmail}/{{email}}")]
    public async Task<Result<AuthUser>> GetUserByEmailAsync([FromRoute] string email, CancellationToken cancel = default)
    {
        try
        {
            cancel.ThrowIfCancellationRequested();

            var identityUser = await _UserManager.FindByEmailAsync(email);
            if (identityUser is null)
            {
                _Logger.LogWarning("Не удалось получить информации о пользователе");
                return Result<AuthUser>.Failure(Errors.Identity.GetUserByEmail.NotFound);
            }

            var roles = await GetUserRolesAsync(identityUser, cancel);

            AuthUser user = new()
            {
                Id = identityUser.Id,
                Email = email,
                UserName = identityUser.UserName,
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
    public async Task<Result<IEnumerable<AuthUser>>> GetAllUsersAsync(CancellationToken cancel = default)
    {
        try
        {
            var listOfAllUsers = await _UserManager.Users.ToListAsync(cancellationToken: cancel);
            var users             = new List<AuthUser>();
            foreach (var user in listOfAllUsers)
            {
                var roles = await GetUserRolesAsync(user, cancel);
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
    public async Task<Result<EditUserNameResponse>> EditUserNameByEmailAsync([FromBody] EditUserNameByEmailRequest editUserRequest, CancellationToken cancel = default)
    {
        try
        {
            cancel.ThrowIfCancellationRequested();

            var identityUser = await _UserManager.FindByEmailAsync(editUserRequest.UserEmail);
            if (identityUser is null)
            {
                _Logger.LogWarning("Не удалось найти пользователя {Email}", editUserRequest.UserEmail);
                return Result<EditUserNameResponse>.Failure(Errors.Identity.EditUserName.NotFound);
            }

            identityUser.UserName = editUserRequest.EditUserNickName;

            cancel.ThrowIfCancellationRequested();

            var identityResult = await _UserManager.UpdateAsync(identityUser);
            if (identityResult.Succeeded)
            {
                var newToken = _authUtilits.CreateSessionToken(
                    identityUser,
                    await _UserManager.GetRolesAsync(identityUser));

                return Result<EditUserNameResponse>.Success(new EditUserNameResponse(newToken));
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
    [HttpDelete($"{AuthApiRoute.DeleteUserByEmail}/{{email}}")]
    public async Task<Result> DeleteUserByEmailAsync([FromRoute] string email, CancellationToken cancel = default)
    {
        try
        {
            cancel.ThrowIfCancellationRequested();

            var identityUser = await _UserManager.FindByEmailAsync(email.ToLower());
            if (identityUser is null)
            {
                _Logger.LogWarning("Не удалось получить информацию о пользователе {Email}", email);
                return Result.Failure(Errors.Identity.DeleteUser.NotFound);
            }

            cancel.ThrowIfCancellationRequested();

            if (_authUtilits.CheckToDeleteSA(identityUser))
            {
                var identityResult = await _UserManager.DeleteAsync(identityUser);
                if (identityResult.Succeeded)
                {
                    return Result.Success();
                }
            }

            _Logger.LogWarning("Не удалось удалить пользователя {Email}", email);
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

    private async Task<List<AuthRole>> GetUserRolesAsync(IdentityUser identityUser, CancellationToken cancel = default)
    {
        try
        {
            cancel.ThrowIfCancellationRequested();

            var userRoles = (await _UserManager.GetRolesAsync(identityUser)).ToHashSet();
            var roles = await _RoleManager.Roles
               .Where(x => userRoles.Contains(x.Name))
               .Select(x => new AuthRole()
                {
                    Id       = x.Id,
                    RoleName = x.Name,
                })
               .ToListAsync(cancellationToken: cancel);
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