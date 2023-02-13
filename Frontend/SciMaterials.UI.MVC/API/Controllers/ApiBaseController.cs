using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace SciMaterials.UI.MVC.API.Controllers;

[ApiController]
public abstract class ApiBaseController<T> : ControllerBase
{
    private ILogger<T> _loggerInstance = default!;

    protected ILogger<T> _logger => _loggerInstance ??= HttpContext.RequestServices.GetService<ILogger<T>>()!;

    protected void LogError(Exception ex, [CallerMemberName] string methodName = null!)
        => _logger.LogError(ex, "Error {error}", methodName);
}