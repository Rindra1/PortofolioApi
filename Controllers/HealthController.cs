using Microsoft.AspNetCore.Mvc;
using PortofolioApi.Domain.Entities;
namespace PortofolioApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet("health")]
    public IActionResult Get() => Ok("API is running");
}
