using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SciMaterials.Auth.Controllers;

/// <summary>
/// Основной контроллер к которому осуществляется ограниченный доступ
/// </summary>
[Authorize(Roles = "sa, admin, user")]
[ApiController]
[Route("[controller]")]
public class MainController : Controller
{
    public MainController()
    { }
    
    [HttpGet("main")]
    public async Task<IActionResult> Main()
    {
        //Some logic...
        return Ok("=>>Main Controller!<<=");
    }
}