using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioTracker.Implementation.Models;
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

        public PortfolioController(IAssetService assetService, IPortfolioService portfolioService, IPortfolioHistoryService portfolioHistoryService, IAssetValueResolver assetValueResolver, MPortfolioDBContext dBContext)
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
            var portfolios = await _dbContext.Portfolio.Include(x => x.Asset).OrderByDescending(x=>x.TotalValue).ToListAsync();

            return portfolios;
        }

        [HttpGet]
        [Route("PortfolioHistory")]
        public IEnumerable<PortfolioHistory> GetPortfolioHistory(DateTime filterDate)
        {
            return _dbContext.PortfolioHistory.Where(x=>x.Date >=filterDate);
        }

        [HttpGet]
        [Route("AccountBalance")]
        public IEnumerable<AccountBalance> GetAccountBalance(DateTime filterDate)
        {
            return _dbContext.AccountBalance.Where(x => x.CreatedOn >= filterDate);
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
            var firstPortfolioHistory = await _dbContext.PortfolioHistory.OrderBy(x => x.Date).FirstAsync();

            var transactions = _dbContext.Transactions.Where(x => x.CreatedOn < firstPortfolioHistory.Date).OrderBy(x => x.CreatedOn).ToList();
            decimal totalValue = 0;
            foreach (var transactionPerDate in transactions.GroupBy(x => x.CreatedOn))
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
            var transactionDate = _dbContext.Transactions.OrderBy(x => x.CreatedOn).First().CreatedOn;
            var transactions = _dbContext.Transactions.ToList();
            var accountbalances = _dbContext.AccountBalance.ToList();

            //ONLY ETFS SUPPORTED ATM and crypto
            var allSupportedAssets = await _dbContext.Assets.Where(x => x.AssetType == AssetType.Etf || x.AssetId == 3 || x.AssetId ==262|| x.AssetType == AssetType.Crypto).ToListAsync();

            var assets = allSupportedAssets.Select(x => x.SymbolForApi).ToArray();
            var assetsLength = assets.Length;
            var skippedrecords = 0;
            var asserthistoryPerSymbols = new List<AssetHistory>();

            while (assetsLength !=0)
            {              
                var result = new List<AssetHistory>();
                if (assetsLength >= 5)
                {
                    result = await _assetValueResolver.GetAssetValueHistory(APIType.YAHOOFINANCE, assets.Skip(skippedrecords).Take(5));
                    skippedrecords += 5;
                    assetsLength -= 5;
                }
                else
                {
                    result = await _assetValueResolver.GetAssetValueHistory(APIType.YAHOOFINANCE, assets.Skip(skippedrecords).Take(assetsLength));
                    skippedrecords += assetsLength;
                    assetsLength -= assetsLength;
                }
                asserthistoryPerSymbols.AddRange(result);
            }

            while (transactionDate < DateTime.Now)
            {
                var allTransactionUntilDate = transactions.Where(x => x.CreatedOn <= transactionDate).ToList();
                var allAccountbalancesTillDate = accountbalances.Where(x => x.CreatedOn <= transactionDate).ToList();
                var totalInvestedForDate = allTransactionUntilDate.Where(x => x.TransactionType != TransactionType.SELL).Sum(x => x.TotalCosts) - allTransactionUntilDate.Where(x => x.TransactionType == TransactionType.SELL).Sum(x => x.TotalCosts);

                //var totalInvestedForDate = allAccountbalancesTillDate.Where(x => x.DepositType == DepositType.DEPOSIT).Sum(x => x.Value) - allAccountbalancesTillDate.Where(x => x.DepositType == DepositType.WITHDRAW).Sum(x => x.Value);

                double totalActualOfAllAssetsValue = 0;

                foreach (var transaction in allTransactionUntilDate.GroupBy(x => x.AssetId))
                {
                    var portfoliohistory = new PortfolioHistory();
                    double totalSharesBought = Convert.ToDouble(transaction.Where(x=> x.TransactionType != TransactionType.SELL).Sum(x => x.AmountOfShares));
                    double totalSharesSold = Convert.ToDouble(transaction.Where(x => x.TransactionType == TransactionType.SELL).Sum(x => x.AmountOfShares));
                    double totalShares = totalSharesBought - totalSharesSold;
                    double totalActualOfAssetValue = 0;
                    var symbolname = allSupportedAssets.FirstOrDefault(x => x.AssetId == transaction.Key)?.SymbolForApi;

                    if (asserthistoryPerSymbols.Exists(x => x.SymbolName == symbolname))
                    {
                        totalActualOfAssetValue = totalShares * asserthistoryPerSymbols.Single(x => x.SymbolName == symbolname).TryGetValueFromDate(transactionDate);
                    }
                    else
                    {
                        //price per share is always the same when dealing with unsupportedsymobls
                        totalActualOfAssetValue = totalShares * Convert.ToDouble(transaction.First().PricePerShare);
                    }

                    totalActualOfAllAssetsValue += totalActualOfAssetValue;

                }

                var portfolioHistory = new PortfolioHistory();
                portfolioHistory.Date = transactionDate;
                portfolioHistory.TotalInvestedPortfolioValue = totalInvestedForDate;
                portfolioHistory.TotalPortfolioValue = Convert.ToDecimal(totalActualOfAllAssetsValue);
                portfolioHistory.Profit = portfolioHistory.TotalPortfolioValue - portfolioHistory.TotalInvestedPortfolioValue;
                if (portfolioHistory.TotalInvestedPortfolioValue != 0)
                {
                    portfolioHistory.Percentage = (portfolioHistory.TotalPortfolioValue - portfolioHistory.TotalInvestedPortfolioValue) / portfolioHistory.TotalInvestedPortfolioValue * 100;
                }

                await _dbContext.PortfolioHistory.AddAsync(portfolioHistory);


                transactionDate = transactionDate.AddDays(1);
            }

            await _dbContext.SaveChangesAsync();
            return Ok();
        }


    }
}
