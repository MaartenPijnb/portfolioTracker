using Microsoft.AspNetCore.Mvc;
using PortfolioTracker.Degiro.Runner.Controller;

namespace PortfolioTracker.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileUploadController : Controller
    {
        private readonly IDegiroController _degiroController;

        public FileUploadController(IDegiroController degiroController)
        {
            _degiroController = degiroController ?? throw new ArgumentNullException(nameof(degiroController));
        }

        [HttpPost]
        public IActionResult Upload(IFormFile file)
        {
            if (file.Length > 0)
            {
                _degiroController.ImportDegiro(new StreamReader(file.OpenReadStream()));
                return Ok();
            }
            return BadRequest();
        }
    }
}
