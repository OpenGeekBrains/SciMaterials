using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace SciMaterials.UI.MVC.Controllers;

public class AccountsController : Controller
{
    private readonly ILogger<AccountsController> _Logger;
    private readonly UserManager<IdentityUser> _UserManager;
    public AccountsController(ILogger<AccountsController> Logger, UserManager<IdentityUser> UserManager)
    {
        _Logger           = Logger;
        _UserManager = UserManager;
    }


    /// <summary>Метод подтверждения почты пользователя, когда пользователь проходит по сформированной ссылке</summary>
    /// <param name="UserId">Идентификатор пользователя в системе</param>
    /// <param name="ConfirmToken">Токен подтверждения</param>
    /// <returns>Status 200 OK.</returns>
    public async Task<IActionResult> ConfirmEmail(string UserId, string ConfirmToken)
    {
        try
        {
            var identity_user = await _UserManager.FindByIdAsync(UserId);
            if (identity_user is null)
            {
                _Logger.Log(LogLevel.Information, "Не удалось найти пользователя в системе {UserId}", UserId);
                return StatusCode(400);
            }

            var identity_result = await _UserManager.ConfirmEmailAsync(identity_user, ConfirmToken);
            if (identity_result.Succeeded)
            {
                return View();
            }

            _Logger.Log(LogLevel.Information, "Не удалось подтвердить email пользователя");
            return StatusCode(500);
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Произошла ошибка при подтверждении почты {Ex}", ex);
            return StatusCode(500);
        }
    }
}