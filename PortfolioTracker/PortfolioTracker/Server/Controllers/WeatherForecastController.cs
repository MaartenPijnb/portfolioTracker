using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using PortfolioTracker.Model;
using PortfolioTracker.Shared;

namespace PortfolioTracker.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly MPortfolioDBContext _dbContext;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, MPortfolioDBContext dBContext)
        {
            _logger = logger;
            _dbContext = dBContext;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            _dbContext.Transactions.Add(new PortfolioTransaction
            {
                CreatedOn = DateTime.Now,
                Symbol = "IWDA"
            });

            _dbContext.SaveChanges();

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}