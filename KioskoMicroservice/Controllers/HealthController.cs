using Microsoft.AspNetCore.Mvc;

namespace KioskoMicroservice.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "OK",
            message = "Servicio funcionando",
            timestamp = DateTime.UtcNow
        });
    }
} 