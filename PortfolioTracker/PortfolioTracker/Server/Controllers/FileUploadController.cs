using Microsoft.AspNetCore.Mvc;
using PortfolioTracker.Bitvavo.Runner.BitvavoController;
using PortfolioTracker.Degiro.Runner.Controller;
using PortfolioTracker.Implementation.Services;

namespace PortfolioTracker.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileUploadController : Controller
    {
        private readonly IDegiroController _degiroController;
        private readonly IBitvavoController _bitvavoController;
        private readonly ILogger<FileUploadController> _logger;
        private readonly IAssetService _assetService;
        private readonly IPortfolioService _portfolioService;
        private readonly IPortfolioHistoryService _portfolioHistoryService;

        public FileUploadController(IDegiroController degiroController, IBitvavoController bitvavoController, ILogger<FileUploadController> logger, IAssetService assetService, IPortfolioService portfolioService, IPortfolioHistoryService portfolioHistoryService)
        {
            _degiroController = degiroController;
            _bitvavoController = bitvavoController;
            _logger = logger;
            _assetService = assetService;
            _portfolioService = portfolioService;
            _portfolioHistoryService = portfolioHistoryService;
        }

        [HttpPost]
        [Route(nameof(UploadDegiro))]
        public IActionResult UploadDegiro(IFormFile file)
        {            
            if (file.Length > 0)
            {
                _degiroController.ImportDegiro(new StreamReader(file.OpenReadStream()));
                return Ok();
            }
            return BadRequest();
        }

        [HttpPost]
        [Route(nameof(UploadBitvavo))]
        public async Task<IActionResult> UploadBitvavo(IFormFile file)
        {
            if (file.Length > 0)
            {
                await _bitvavoController.ImportBitvavo(new StreamReader(file.OpenReadStream()));
                return Ok();
            }
            return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            await _assetService.UpdateAssets();
            await _portfolioService.UpdatePortfolio();
            await _portfolioHistoryService.CreatePortfolioHistory();
            return Ok();
        }
    }
}
