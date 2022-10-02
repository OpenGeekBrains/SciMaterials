using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SciMaterials.Auth.Data.AuthRepository;
using SciMaterials.Auth.Requests;

namespace SciMaterials.Auth.Controllers;

/// <summary>
/// Контроллер администратора для управления пользователями
/// </summary>
[Authorize(Roles = "sa, admin")]
[ApiController]
[Route("[controller]")]
public class AdminUsersController : Controller
{
    private readonly IUsersRepository _usersRepository;
    public AdminUsersController(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }

    [HttpPost("CreateUser")]
    public async Task<IActionResult> CreateUser([FromBody] UserRequest create)
    {
        //Это временная проверка
        if (!string.IsNullOrEmpty(create.Email) || 
            !string.IsNullOrEmpty(create.Password) || 
            !string.IsNullOrEmpty(create.PhoneNumber))
        {
            var result = await _usersRepository.CreateUserAsync(create);
            if (result.Succeeded)
            {
                return Ok("Пользователь успешно создан в системе");
            }

            return BadRequest("Не удалось создать пользователя в системе");
        }

        return BadRequest("Некорректно введены данные пользователя");
    }
    
    [HttpPost("GetUserByEmail")]
    public async Task<IActionResult> GetUserByEmail(string email)
    {
        //Это временная проверка
        if (!string.IsNullOrEmpty(email))
        {
            var result = await _usersRepository.GetUserByEmailAsync(email);
            if (result is not null)
            {
                return Ok(result);
            }

            return BadRequest("Не удалось найти пользователя по данному адресу почты");
        }

        return BadRequest();
    }
    
    [HttpPost("GetAllUsers")]
    public async Task<IActionResult> GetAllUsers()
    {
        var result = await _usersRepository.GetAllUsersAsync();
        
        if (result is not null)
        {
            return Ok(result);
        }

        return BadRequest("Не удалось получить список пользователей");
    }
    
    [HttpPost("EditUserByEmail")]
    public async Task<IActionResult> EditUserByEmail(string email, UserRequest edit)
    {
        //Это временная проверка
        if (!string.IsNullOrEmpty(email) || edit is not null)
        {
            var result = await _usersRepository.EditUserByEmailAsync(email, edit);
            return Ok(result);
        }

        return BadRequest("Некорректно введены данные пользователя");
    }
    
    [HttpDelete("DeleteUserByEmail")]
    public async Task<IActionResult> DeleteUserByEmail(string email)
    {
        //Это временная проверка
        if (!string.IsNullOrEmpty(email))
        {
            var result = await _usersRepository.DeleteUserByEmailAsync(email);
            return Ok(result);
        }

        return BadRequest("Некорректно введены данные пользователя");
    }
}