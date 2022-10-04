using Microsoft.AspNetCore.Mvc;

namespace SciMaterials.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestSampleController : ControllerBase
{
    [HttpGet]
    public IActionResult Check()
    {
        return Ok();
    }
}