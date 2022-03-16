using Microsoft.AspNetCore.Mvc;
using PortfolioTracker.Implementation.Services;

namespace PortfolioTracker.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PortfolioController : Controller
    {
        private readonly IAssetService _assetService;
        private readonly IPortfolioService _portfolioService;
        private readonly IPortfolioHistoryService _portfolioHistoryService;

        public PortfolioController(IAssetService assetService, IPortfolioService portfolioService, IPortfolioHistoryService portfolioHistoryService)
        {
            _assetService = assetService;
            _portfolioService = portfolioService;
            _portfolioHistoryService = portfolioHistoryService;
        }


        [HttpGet]
        [Route("UpdateAssets")]
        public async Task<IActionResult> UpdateAssets()
        {
            await _assetService.UpdateAssets();
            await _portfolioService.UpdatePortfolio();
            return Ok();
        }

        [HttpGet]
        [Route("CreatePortfolioHistory")]

        public async Task<IActionResult> CreatePortfolioHistory()
        {
            await _assetService.UpdateAssets();
            await _portfolioService.UpdatePortfolio();
            await _portfolioHistoryService.CreatePortfolioHistory();
            return Ok();
        }
    }
}
