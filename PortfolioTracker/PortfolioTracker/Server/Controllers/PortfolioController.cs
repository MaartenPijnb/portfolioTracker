using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioTracker.Implementation.Resolvers;
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
        private readonly IAssetValueResolver _assetValueResolver;
        private readonly MPortfolioDBContext _dbContext;

        public PortfolioController(IAssetService assetService, IPortfolioService portfolioService, IPortfolioHistoryService portfolioHistoryService, IAssetValueResolver assetValueResolver , MPortfolioDBContext dBContext)
        {
            _assetService = assetService;
            _portfolioService = portfolioService;
            _portfolioHistoryService = portfolioHistoryService;
            _assetValueResolver = assetValueResolver;
            _dbContext = dBContext;
        }

        [HttpGet]
        public async Task<IEnumerable<Portfolio>> Get()
        {
            var portfolios = await _dbContext.Portfolio.Include(x=>x.Asset).ToListAsync();

            return portfolios; 
        }

        [HttpGet]
        [Route("PortfolioHistory")]
        public IEnumerable<PortfolioHistory> GetPortfolioHistory()
        {
            return _dbContext.PortfolioHistory;
        }

        [HttpPost]
        [Route("UpdateAssets")]
        public async Task<IActionResult> UpdateAssets()
        {
            await _assetService.UpdateAssets();
            await _portfolioService.UpdatePortfolio();
            return Ok();
        }

        [HttpPost]
        [Route("CreatePortfolioHistory")]

        public async Task<IActionResult> CreatePortfolioHistory()
        {
            await _assetService.UpdateAssets();
            await _portfolioService.UpdatePortfolio();
            await _portfolioHistoryService.CreatePortfolioHistory();
            return Ok();
        }

        [HttpPost]
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

        [HttpPost]
        [Route("CreatePortfolioHistoryOnceWithRealCalcluation")]

        public async Task<IActionResult> CreatePortfolioHistoryOnceWitRealCalcluation()
        {
            var firstPortfolioHistory = await _dbContext.PortfolioHistory.OrderBy(x => x.Date).FirstOrDefaultAsync();
            var firstCalculationDate = firstPortfolioHistory != null ? firstPortfolioHistory.Date : DateTime.Now;

            var transactions = _dbContext.Transactions.Where(x => x.CreatedOn < firstCalculationDate).OrderBy(x => x.CreatedOn).ToList();      

            var transactionDate = transactions.First().CreatedOn;
            var assets = new string[] { "IWDA.AS", "IEMA.AS"};
            var asserthistoryPerSymbols = await _assetValueResolver.GetAssetValueHistory(APIType.YAHOOFINANCE, assets);

            
            while (transactionDate < firstPortfolioHistory.Date)
            {
                if (asserthistoryPerSymbols[0].AssetHistoryValues.ContainsKey(transactionDate) && asserthistoryPerSymbols[1].AssetHistoryValues.ContainsKey(transactionDate))
                {
                    var allTransactionUntilDate = transactions.Where(x => x.CreatedOn <= transactionDate).ToList();
                    var totalInvestedForDate = allTransactionUntilDate.Sum(x => x.TotalCosts);
                    //always iwda or iema
                    double totalActualOfAllAssetsValue = 0;

                    foreach (var transaction in allTransactionUntilDate.GroupBy(x => x.AssetId))
                    {
                        var portfoliohistory = new PortfolioHistory();
                        double totalShares = Convert.ToDouble(transaction.Sum(x => x.AmountOfShares));
                        double totalActualOfAssetValue = 0;

                        if (transaction.Key == 1)
                        {
                            totalActualOfAssetValue = totalShares *  asserthistoryPerSymbols[0].AssetHistoryValues[transactionDate];
                        }
                        else if (transaction.Key == 2)
                        {
                            totalActualOfAssetValue = totalShares * asserthistoryPerSymbols[1].AssetHistoryValues[transactionDate];
                        }
                        totalActualOfAllAssetsValue += totalActualOfAssetValue;
                    }
                    var portfolioHistory = new PortfolioHistory();
                    portfolioHistory.Date = transactionDate;
                    portfolioHistory.TotalInvestedPortfolioValue = totalInvestedForDate;
                    portfolioHistory.TotalPortfolioValue = Convert.ToDecimal(totalActualOfAllAssetsValue);
                    portfolioHistory.Profit = portfolioHistory.TotalPortfolioValue - portfolioHistory.TotalInvestedPortfolioValue;
                    portfolioHistory.Percentage = (portfolioHistory.TotalPortfolioValue - portfolioHistory.TotalInvestedPortfolioValue) / portfolioHistory.TotalInvestedPortfolioValue * 100;
                    
                    await _dbContext.PortfolioHistory.AddAsync(portfolioHistory);
                }

                transactionDate = transactionDate.AddDays(1);              
            }
          
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
