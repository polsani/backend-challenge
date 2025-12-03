using Microsoft.AspNetCore.Mvc;

namespace Challenge.Api.Controllers.v1;

[ApiController]
[Route("/api/v1/[controller]")]
public class TransactionController : ControllerBase
{
    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<string> Get()
    {
        return Summaries;
    }
}