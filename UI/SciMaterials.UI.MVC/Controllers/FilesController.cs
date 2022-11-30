using Microsoft.AspNetCore.Mvc;
using SciMaterials.Contracts.API.Settings;

namespace SciMaterials.UI.MVC.Controllers;

public class FilesController : Controller
{
    private readonly ILogger<FilesController> _Logger;
    private static string? _Message;
    private readonly ApiSettings _ApiSettings;

    public FilesController(ApiSettings ApiSettings, ILogger<FilesController> Logger)
    {
        _Logger = Logger;

        _ApiSettings = ApiSettings;
        if (string.IsNullOrEmpty(ApiSettings.BasePath))
            throw new ArgumentNullException(nameof(ApiSettings));
    }

    public IActionResult Index()
    {
        ViewBag.Message = _Message;
        return View();
    }
}
