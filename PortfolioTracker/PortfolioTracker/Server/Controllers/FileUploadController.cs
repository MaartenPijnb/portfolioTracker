using Microsoft.AspNetCore.Mvc;
using PortfolioTracker.Degiro.Runner.Controller;
using PortfolioTracker.Implementation.Services;

namespace PortfolioTracker.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileUploadController : Controller
    {
        private readonly IDegiroController _degiroController;
        private readonly ILogger<FileUploadController> _logger;
        private readonly IAssetService _assetService;
        private readonly IPortfolioService _portfolioService;

        public FileUploadController(IDegiroController degiroController, ILogger<FileUploadController> logger, IAssetService assetService, IPortfolioService portfolioService)
        {
            _degiroController = degiroController ?? throw new ArgumentNullException(nameof(degiroController));
            _logger = logger;
            _assetService = assetService;
            _portfolioService = portfolioService;
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

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            await _assetService.UpdateAssets();
            await _portfolioService.UpdatePortfolio();
            return Ok();
        }
    }
}
