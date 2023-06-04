using Microsoft.AspNetCore.Mvc;

namespace SciMaterials.Materials.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet("check")]
    public IActionResult Check() => Ok();
}