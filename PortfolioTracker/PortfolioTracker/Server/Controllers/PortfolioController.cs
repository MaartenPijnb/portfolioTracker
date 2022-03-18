using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioTracker.Implementation.Services;
using PortfolioTracker.Model;

namespace PortfolioTracker.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PortfolioController : Controller
    {
        private readonly IAssetService _assetService;
        private readonly IPortfolioService _portfolioService;
        private readonly IPortfolioHistoryService _portfolioHistoryService;
        private readonly MPortfolioDBContext _dbContext;

        public PortfolioController(IAssetService assetService, IPortfolioService portfolioService, IPortfolioHistoryService portfolioHistoryService, MPortfolioDBContext dBContext)
        {
            _assetService = assetService;
            _portfolioService = portfolioService;
            _portfolioHistoryService = portfolioHistoryService;
            _dbContext = dBContext;
        }

        [HttpGet]
        public async Task<IEnumerable<Portfolio>> Get()
        {
            return await _dbContext.Portfolio.Include(x=>x.Asset).ToListAsync();
        }

        [HttpGet]
        [Route("PortfolioHistory")]
        public IEnumerable<PortfolioHistory> GetPortfolioHistory()
        {
            return _dbContext.PortfolioHistory;
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

        [HttpGet]
        [Route("CreatePortfolioHistoryOnceWithoutRealCalcluation")]

        public async Task<IActionResult> CreatePortfolioHistoryOnceWithoutRealCalcluation()
        {
            var firstPortfolioHistory =await _dbContext.PortfolioHistory.OrderBy(x => x.Date).FirstAsync();

            var transactions = _dbContext.Transactions.Where(x => x.CreatedOn < firstPortfolioHistory.Date).OrderBy(x => x.CreatedOn).ToList();
            decimal totalValue = 0;
            foreach (var transactionPerDate in transactions.GroupBy(x=>x.CreatedOn))
            {
                totalValue += transactionPerDate.Sum(x => x.TotalCosts);
                var portfolioHistory = new PortfolioHistory
                {
                    Date = transactionPerDate.Key,
                    Percentage = 0,
                    Profit = 0,
                    TotalInvestedPortfolioValue = totalValue,
                    TotalPortfolioValue = totalValue
                };
                await _dbContext.PortfolioHistory.AddAsync(portfolioHistory);
            }
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
