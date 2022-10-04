using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SciMaterials.Auth.Requests;

namespace SciMaterials.Auth.Controllers;

/// <summary>
/// Контроллер для регистрации и авторизации в системе
/// </summary>
[ApiController]
[Route("[controller]")]
public class AccountController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly ILogger<AccountController> _logger;
    
    public AccountController(
        UserManager<IdentityUser> userManager, 
        SignInManager<IdentityUser> signInManager, 
        RoleManager<IdentityRole> roleManager, 
        ILogger<AccountController> logger, 
        IHttpContextAccessor contextAccessor)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _logger = logger;
        _contextAccessor = contextAccessor;
    }

    [AllowAnonymous]
    [HttpPost("Register")]
    public async Task<IActionResult> RegisterAsync([FromBody] UserRequest registerUserRequest)
    {
        //Это временная проверка
        if (!string.IsNullOrEmpty(registerUserRequest.Email) || 
            !string.IsNullOrEmpty(registerUserRequest.Password) ||
            !string.IsNullOrEmpty(registerUserRequest.PhoneNumber))
        {
            try
            {
                var user = new IdentityUser()
                {
                    Email = registerUserRequest.Email,
                    UserName = registerUserRequest.Email,
                    PhoneNumber = registerUserRequest.PhoneNumber,
                };

                var result = await _userManager.CreateAsync(user, registerUserRequest.Password);
                await _userManager.AddToRoleAsync(user, "user");
                await _signInManager.SignInAsync(user, false);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");
            }

            return BadRequest("Пользователя не удалось зарегистрировать");
        }
        
        return BadRequest("Некорректно введены данные пользователя");
    }
    
    [AllowAnonymous]
    [HttpPost("Login")]
    public async Task<IActionResult> LoginAsync(string email, string password)
    {
        //Это временная проверка
        if (!string.IsNullOrEmpty(email) || !string.IsNullOrEmpty(password))
        {
            try
            {
                var identityResult = await _userManager.FindByEmailAsync(email);
                if (identityResult is not null)
                {
                    var signInResult = await _signInManager.PasswordSignInAsync(
                        userName: email, 
                        password: password, 
                        isPersistent: true, 
                        lockoutOnFailure: false);
            
                    if (signInResult.Succeeded)
                    {
                        return Ok(signInResult);
                    }

                    return BadRequest("Не удалось авторизовать пользователя");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");
            }

            return BadRequest("Не удалось авторизовать пользователя");
        }

        return BadRequest("Некорректно введены данные пользователя");
    }
    
    [AllowAnonymous]
    [HttpPost("Logout")]
    public async Task<IActionResult> LogoutAsync()
    {
        try
        {
            await _signInManager.SignOutAsync();
            return Ok("Пользователь вышел из системы");
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex}");
            return BadRequest("Не удалось выйти из системы");
        }
    }

    [Authorize(Roles = "admin, user")]
    [HttpPost("ChangePassword")]
    public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword)
    {
        if (!string.IsNullOrEmpty(oldPassword) || !string.IsNullOrEmpty(newPassword))
        {
            try
            {
                var currentUserName = _contextAccessor.HttpContext.User.Identity.Name;
                var identityUser = await _userManager.FindByNameAsync(currentUserName);
                if (!string.IsNullOrEmpty(currentUserName) || identityUser is not null)
                {
                    var identityResult = await _userManager.ChangePasswordAsync(
                        identityUser, 
                        oldPassword, 
                        newPassword);
                    if (identityResult.Succeeded)
                    {
                        return Ok(identityResult);
                    }

                    return BadRequest("Не удалось изменить пароль");
                }

                return BadRequest("Не удалось получить информацию о пользователе");
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");
                return BadRequest("Произошла ошибка при смене пароля");
            }
        }

        return BadRequest("Некорректно введены данные");
    }

    [Authorize(Roles = "admin")]
    [HttpPost("CreateRole")]
    public async Task<IActionResult> CreateRoleAsync(string roleName)
    {
        //Это временная проверка
        if (!string.IsNullOrEmpty(roleName))
        {
            try
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");
                return BadRequest("Произошла ошибка при создании роли");
            }
        }

        return BadRequest("Некорректно введены данные");
    }
    
    [Authorize(Roles = "admin")]
    [HttpGet("GetAllRoles")]
    public async Task<IActionResult> GetAllRolesAsync()
    {
        try
        {
            var result = await _roleManager.Roles.ToListAsync();
            if (result is not null)
            {
                return Ok(result);
            }

            return BadRequest("Не удалось получить список ролей");
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex}");
            return BadRequest("Произошла ошибка при запросе ролей");
        }
    }
    
    [Authorize(Roles = "admin")]
    [HttpGet("GetRoleById")]
    public async Task<IActionResult> GetRoleByIdAsync(string id)
    {
        if (!string.IsNullOrEmpty(id))
        {
            try
            {
                var result = await _roleManager.FindByIdAsync(id);
                if (result is not null)
                {
                    return Ok(result);
                }

                return BadRequest("Не удалось получить роль");
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");
                return BadRequest("Произошла ошибка при запросе роли");
            }
        }
        
        return BadRequest("Некорректно введены данные пользователя");
    }
    
    [Authorize(Roles = "admin")]
    [HttpPost("EditRoleById")]
    public async Task<IActionResult> EditRoleByIdAsync(string id, string roleName)
    {
        //Это временная проверка
        if (!string.IsNullOrEmpty(id) || !string.IsNullOrEmpty(roleName))
        {
            try
            {
                var foundRole = await _roleManager.FindByIdAsync(id);
                foundRole.Name = roleName;
            
                var result = await _roleManager.UpdateAsync(foundRole);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");
                return BadRequest("Произошла ошибка при редактировании роли");
            }
        }
        
        return BadRequest("Некорректно введены данные пользователя");
    }
    
    [Authorize(Roles = "admin")]
    [HttpDelete("DeleteRoleById")]
    public async Task<IActionResult> DeleteRoleByIdAsync(string id)
    {
        if (!string.IsNullOrEmpty(id))
        {
            try
            {
                var result = await _roleManager.FindByIdAsync(id);
                if (result is not null)
                {
                    var userRole = await _roleManager.DeleteAsync(result);
                    return Ok(userRole);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");
                return BadRequest("Произошла ошибка при удалении роли");
            }
        }
        
        return BadRequest("Некорректно введены данные пользователя");
    }
    
    [Authorize(Roles = "admin")]
    [HttpPost("AddUserRole")]
    public async Task<IActionResult> AddUserRoleAsync(string email, string roleName)
    {
        //Это временная проверка
        if (!string.IsNullOrEmpty(email) || !string.IsNullOrEmpty(roleName))
        {
            try
            {
                var foundUser = await _userManager.FindByEmailAsync(email);
                var userRoles = await _userManager.GetRolesAsync(foundUser);
            
                var roles = await _roleManager.Roles.ToListAsync();
                if (!userRoles.Contains(roleName))
                {
                    var isRoleContainsInSystem = roles.Select(x => x.Name.Contains(roleName.ToLower()));
                    foreach (var isRole in isRoleContainsInSystem)
                    {
                        if (isRole)
                        {
                            var roleAddedResult = await _userManager.AddToRoleAsync(foundUser, roleName.ToLower());
                            if (roleAddedResult.Succeeded)
                            {
                                return Ok(roleAddedResult);
                            }
                        }
                    }
                }

                return BadRequest("Некорректно введены данные");
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");
                return BadRequest("Произошла ошибка при добавлении роли пользователю");
            }
        }

        return BadRequest("Некорректно введены данные");
    }
    
    [Authorize(Roles = "admin")]
    [HttpDelete("DeleteUserRole")]
    public async Task<IActionResult> DeleteUserRoleAsync(string email, string roleName)
    {
        //Это временная проверка
        if (!string.IsNullOrEmpty(email) || !string.IsNullOrEmpty(roleName))
        {
            try
            {
                var foundUser = await _userManager.FindByEmailAsync(email);
                var userRoles = await _userManager.GetRolesAsync(foundUser);
            
                var roles = await _roleManager.Roles.ToListAsync();
                if (userRoles.Contains(roleName))
                {
                    var isRoleContainsInSystem = roles.Select(x => x.Name.Contains(roleName));
                    foreach (var isRole in isRoleContainsInSystem)
                    {
                        if (isRole)
                        {
                            var roleRemovedResult = await _userManager.RemoveFromRoleAsync(foundUser, roleName);
                            if (roleRemovedResult.Succeeded)
                            {
                                return Ok(roleRemovedResult);
                            }
                        }
                    }
                }

                return BadRequest("Некорректно введены данные");
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");
                return BadRequest("Произошла ошибка при удалении роли пользователю");
            }
        }

        return BadRequest("Некорректно введены данные");
    }

    [Authorize(Roles = "admin")]
    [HttpGet("ListOfUserRoles")]
    public async Task<IActionResult> ListOfUserRolesAsync(string email)
    {
        if (!string.IsNullOrEmpty(email))
        {
            try
            {
                var foundUser = await _userManager.FindByEmailAsync(email);
                if (foundUser is not null)
                {
                    var userRoles = await _userManager.GetRolesAsync(foundUser);
                    if (userRoles is not null)
                    {
                        return Ok(userRoles.ToList());
                    }

                    return BadRequest("Не удалось получить список ролей");
                }

                return BadRequest("Данного пользователя нет в системе, либо некорректно введены данные пользователя");
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");
                return BadRequest("Произошла ошибка при получении списка ролей пользователей");
            }
        }

        return BadRequest("Некорректно введены данные");
    }
    
    [Authorize(Roles = "admin")]
    [HttpPost("CreateUser")]
    public async Task<IActionResult> CreateUserAsync([FromBody] UserRequest create)
    {
        //Это временная проверка
        if (!string.IsNullOrEmpty(create.Email) || 
            !string.IsNullOrEmpty(create.Password) || 
            !string.IsNullOrEmpty(create.PhoneNumber))
        {
            await RegisterAsync(create);
            return Ok("Пользователь успешно создан в системе");
        }

        return BadRequest("Некорректно введены данные пользователя");
    }
    
    [Authorize(Roles = "admin")]
    [HttpPost("GetUserByEmail")]
    public async Task<IActionResult> GetUserByEmailAsync(string email)
    {
        //Это временная проверка
        if (!string.IsNullOrEmpty(email))
        {
            try
            {
                var result = await _userManager.FindByEmailAsync(email);
                if (result is not null)
                {
                    return Ok(result);
                }

                return BadRequest("Не удалось получить информации о пользователе");
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");
                return BadRequest("Произошла ошибка при получении пользователей");
            }
        }

        return BadRequest("Некорректно введены данные");
    }
    
    [Authorize(Roles = "admin")]
    [HttpPost("GetAllUsers")]
    public async Task<IActionResult> GetAllUsersAsync()
    {
        try
        {
            var listOfAllUsers = await _userManager.Users.ToListAsync();
            return Ok(listOfAllUsers);
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex}");
            return BadRequest("Произошла ошибка при получении пользователя");
        }
    }
    
    [Authorize(Roles = "admin")]
    [HttpPost("EditUserByEmail")]
    public async Task<IActionResult> EditUserByEmailAsync(string email, UserRequest editUserRequest)
    {
        //Это временная проверка
        if (!string.IsNullOrEmpty(email) || editUserRequest is not null)
        {
            try
            {
                var foundUser = await _userManager.FindByEmailAsync(email);
                if (foundUser is not null)
                {
                    foundUser.Email = editUserRequest.Email;
                    foundUser.PhoneNumber = editUserRequest.PhoneNumber;
                    foundUser.UserName = editUserRequest.Email;

                    var result = await _userManager.UpdateAsync(foundUser);
                    if (result.Succeeded)
                    {
                        return Ok(result);
                    }

                    return BadRequest("Не удалось обновить информацию пользователя");
                }

                return BadRequest("Не удалось найти данного пользователя или некорректно введены данные пользователя");
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");
                return BadRequest("Произошла ошибка при обновлении информации о пользователе");
            }
        }
        
        return BadRequest("Некорректно введены данные пользователя");
    }
    
    [Authorize(Roles = "admin")]
    [HttpDelete("DeleteUserByEmail")]
    public async Task<IActionResult> DeleteUserByEmail(string email)
    {
        //Это временная проверка
        if (!string.IsNullOrEmpty(email))
        {
            try
            {
                var foundUser = await _userManager.FindByEmailAsync(email);
                if (foundUser is not null)
                {
                    var result = await _userManager.DeleteAsync(foundUser);
                    if (result.Succeeded)
                    {
                        return Ok(result);
                    }

                    return BadRequest("Не удалось удалить пользователя");
                }

                return BadRequest("Не удалось получить информацию о пользователе");
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");
                return BadRequest("Произошла ошибка при удалении пользователя");
            }
        }

        return BadRequest("Некорректно введены данные пользователя");
    }
}