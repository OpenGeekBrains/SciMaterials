using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SciMaterials.Auth.Data.AuthRepository;
using SciMaterials.Auth.Requests;

namespace SciMaterials.Auth.Controllers;

/// <summary>
/// Контроллер для регистрации и авторизации в системе
/// </summary>
[ApiController]
[Route("[controller]")]
public class AccountController : Controller
{
    private readonly IUsersRepository _usersRepository;
    
    public AccountController(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }

    [AllowAnonymous]
    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] UserRequest userRequest)
    {
        //Это временная проверка
        if (!string.IsNullOrEmpty(userRequest.Email) || 
            !string.IsNullOrEmpty(userRequest.Password) ||
            !string.IsNullOrEmpty(userRequest.PhoneNumber))
        {
            var result = await _usersRepository.CreateUserAsync(userRequest);
            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest("Пользователя не удалось зарегистрировать");
        }
        
        return BadRequest("Некорректно введены данные пользователя");
    }
    
    [AllowAnonymous]
    [HttpPost("Login")]
    public async Task<IActionResult> Login(string email, string password)
    {
        //Это временная проверка
        if (!string.IsNullOrEmpty(email) || !string.IsNullOrEmpty(password))
        {
            var result = await _usersRepository.Login(email, password);
            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest("Не удалось авторизовать пользователя");
        }

        return BadRequest("Некорректно введены данные пользователя");
    }
    
    [AllowAnonymous]
    [HttpPost("Logout")]
    public async Task<IActionResult> Logout()
    {
        var result = await _usersRepository.Logout();
        if (result.IsCompletedSuccessfully)
        {
            return Ok("Пользователь вышел из системы");
        }

        return BadRequest("Произошла ошибка авторизации");
    }
}