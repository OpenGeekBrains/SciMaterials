using Microsoft.AspNetCore.Mvc;

namespace SciMaterials.UI.MVC.Controllers;

public class HomeController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}