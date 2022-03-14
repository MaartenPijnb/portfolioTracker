using Microsoft.AspNetCore.Mvc;
using PortfolioTracker.Degiro.Runner.Controller;

namespace PortfolioTracker.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileUploadController : Controller
    {
        private readonly IDegiroController _degiroController;
        private readonly ILogger<FileUploadController> _logger;

        public FileUploadController(IDegiroController degiroController, ILogger<FileUploadController> logger)
        {
            _degiroController = degiroController ?? throw new ArgumentNullException(nameof(degiroController));
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Upload(IFormFile file)
        {
            _logger.LogWarning("Maarten zegt hoi");
            if (file.Length > 0)
            {
                _degiroController.ImportDegiro(new StreamReader(file.OpenReadStream()));
                return Ok();
            }
            return BadRequest();
        }
    }
}
