using Dapr.Client;
using Microsoft.AspNetCore.Mvc;

namespace DaprCounterASPNET.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }


    [HttpGet("counter")]
    public async Task<IActionResult> GetCounter([FromServices] DaprClient daprClient)
    {
        int counter = await daprClient.GetStateAsync<int>("statestore", "counter");
        return Ok(counter);
    }

    [HttpPut("counter")]
    public async Task<IActionResult> PutCounter([FromServices] DaprClient daprClient)
    {
        var counter = await daprClient.GetStateEntryAsync<int>("statestore", "counter");

        counter.Value += 1;

        if (await counter.TrySaveAsync())
        {
            return Ok(counter.Value);
        }
        else
        {
            return BadRequest();
        }
    }
}
