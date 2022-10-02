using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SciMaterials.Auth.Data.AuthRepository;
using SciMaterials.Auth.Requests;

namespace SciMaterials.Auth.Controllers;

/// <summary>
/// Контроллер для администратора для управлением ролями пользователей
/// </summary>
[Authorize(Roles = "sa, admin")]
[ApiController]
[Route("[controller]")]
public class AdminRolesController : Controller
{
    private readonly IUserRolesRepository _userRolesRepository;
    
    public AdminRolesController(IUserRolesRepository userRolesRepository)
    {
        _userRolesRepository = userRolesRepository;
    }
    
    [HttpPost("CreateRole")]
    public async Task<IActionResult> CreateRole(string roleName)
    {
        //Это временная проверка
        if (!string.IsNullOrEmpty(roleName))
        {
            var result = await _userRolesRepository.CreateRoleAsync(roleName);
            if (result.Succeeded)
            {
                return Ok(result);
            }
        }

        return BadRequest("Произошла ошибка при создании роли");
    }
    
    [HttpGet("GetAllRoles")]
    public async Task<IActionResult> GetAllRoles()
    {
        var result = await _userRolesRepository.GetAllRolesAsync();
        if (result is not null)
        {
            return Ok(result);
        }

        return BadRequest("Произошла ошибка при запросе ролей");
    }
    
    [HttpGet("GetRoleById")]
    public async Task<IActionResult> GetRoleById(string id)
    {
        if (!string.IsNullOrEmpty(id))
        {
            var result = await _userRolesRepository.GetRoleByIdAsync(id);
            if (result is not null)
            {
                return Ok(result);
            }

            return BadRequest("Произошла ошибка при запросе роли");
        }
        
        return BadRequest("Некорректно введены данные пользователя");
    }
    
    [HttpPost("EditRoleById")]
    public async Task<IActionResult> EditRoleById(string id, RoleRequest roleRequest)
    {
        //Это временная проверка
        if (!string.IsNullOrEmpty(id) || roleRequest is not null)
        {
            var result = await _userRolesRepository.EditRoleByIdAsync(id, roleRequest);
            if (result is not null)
            {
                return Ok(result);
            }

            return BadRequest("Произошла ошибка при редактировании роли");
        }
        
        return BadRequest("Некорректно введены данные пользователя");
    }
    
    [HttpDelete("DeleteRoleById")]
    public async Task<IActionResult> DeleteRoleById(string id)
    {
        if (!string.IsNullOrEmpty(id))
        {
            var result = await _userRolesRepository.DeleteRoleByIdAsync(id);
            if (result is not null)
            {
                return Ok(result);
            }

            return BadRequest("Произошла ошибка при удалении роли");
        }
        
        return BadRequest("Некорректно введены данные пользователя");
    }
    
    [HttpPost("AddUserRole")]
    public async Task<IActionResult> AddUserRole(string email, string roleName)
    {
        //Это временная проверка
        if (!string.IsNullOrEmpty(email) || !string.IsNullOrEmpty(roleName))
        {
            var result = await _userRolesRepository.AddUserRole(email, roleName.ToLower());
            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest("Изменить роль пользователю не удалось");
        }

        return BadRequest("Некорректно введены данные");
    }
    
    [HttpDelete("DeleteUserRole")]
    public async Task<IActionResult> DeleteUserRole(string email, string roleName)
    {
        //Это временная проверка
        if (!string.IsNullOrEmpty(email) || !string.IsNullOrEmpty(roleName))
        {
            var result = await _userRolesRepository.DeleteUserRole(email, roleName.ToLower());
            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest("Изменить роль пользователю не удалось");
        }

        return BadRequest("Некорректно введены данные");
    }

    [HttpGet("ListOfUserRoles")]
    public async Task<IActionResult> ListOfUserRoles(string email)
    {
        if (!string.IsNullOrEmpty(email))
        {
            var result = await _userRolesRepository.ListOfUserRoles(email);
            if (result is not null)
            {
                return Ok(result);
            }

            return BadRequest("Не удалось найти необходимого пользователя");
        }

        return BadRequest("Некорректно введены данные");
    }
}