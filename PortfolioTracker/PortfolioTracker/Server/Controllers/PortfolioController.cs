using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PortfolioTracker.Implementation.Models;
using PortfolioTracker.Implementation.Resolvers;
using PortfolioTracker.Implementation.Services;
using PortfolioTracker.Model;
using System.Text.Json.Serialization;

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
        [Route("{UserId}")]
        public async Task<IEnumerable<Portfolio>> Get(int UserId)
        {
            var portfolios = await _dbContext.Portfolio.Where(x=>x.UserID==UserId).Include(x => x.Asset).OrderByDescending(x=>x.TotalValue).ToListAsync();

            return portfolios;
        }

        [HttpGet]
        [Route("PortfolioHistory")]
        public IEnumerable<PortfolioHistory> GetPortfolioHistory(DateTime filterDate, long userId)
        {
            return _dbContext.PortfolioHistory.Where(x=>x.Date >=filterDate && x.UserID == userId);
        }

        [HttpGet]
        [Route("AccountBalance")]
        public IEnumerable<AccountBalance> GetAccountBalance(DateTime filterDate)
        {
            return _dbContext.AccountBalance.Where(x => x.CreatedOn >= filterDate);
        }


        [HttpPost]
        [Route("UpdateAssets/{userId}")]
        public async Task<IActionResult> UpdateAssets(long userId)
        {
            await _assetService.UpdateAssets();
            await _portfolioService.UpdatePortfolio(userId);
            return Ok();
        }

        [HttpPost]
        [Route("DeleteAccount/{userId}")]
        public async Task DeleteAccountPortfolio(long userId)
        {
            await _dbContext.Database.ExecuteSqlRawAsync($"delete from AccountBalance where UserId= {userId}");
            await _dbContext.Database.ExecuteSqlRawAsync($"delete from Portfolio where UserId = {userId}");
            await _dbContext.Database.ExecuteSqlRawAsync($"delete from PortfolioHistory where UserId = {userId}");
            await _dbContext.Database.ExecuteSqlRawAsync($"delete from Transactions where UserId = {userId}");
        }

        [HttpPost]
        [Route("CreatePortfolioHistory/{userId}")]

        public async Task<IActionResult> CreatePortfolioHistory(long userId)
        {
            await _assetService.UpdateAssets();
            await _portfolioService.UpdatePortfolio(userId);
            await _portfolioHistoryService.CreatePortfolioHistory(userId);
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
        [Route("CreatePortfolioHistoryOnceWithRealCalcluation/{userId}")]

        public async Task<IActionResult> CreatePortfolioHistoryOnceWitRealCalcluation(long userId)
        {
            var transactionDate = _dbContext.Transactions.Where(x=>x.UserID==userId).OrderBy(x => x.CreatedOn).First().CreatedOn;
            var transactions = _dbContext.Transactions.Where(x => x.UserID == userId).ToList();
            var accountbalances = _dbContext.AccountBalance.Where(x => x.UserID == userId).ToList();

            //ONLY ETFS SUPPORTED ATM and crypto
            var allSupportedAssets = await _dbContext.Assets.Where(x => x.AssetType == AssetType.Etf || x.AssetId == 3 || x.AssetId ==262|| x.AssetType == AssetType.Crypto).ToListAsync();

            var assets = allSupportedAssets.Select(x => x.SymbolForApi).ToArray();
            var assetsLength = assets.Length;
            var skippedrecords = 0;
            var asserthistoryPerSymbols = new List<AssetHistory>();

            while (assetsLength != 0)
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
            ////12 mei?
            //transactionDate = new DateTime(2022, 5, 12);
            //string readText = System.IO.File.ReadAllText("C:\\portfoliofiles\\test.json");
            // asserthistoryPerSymbols = JsonConvert.DeserializeObject<List<AssetHistory>>(readText);
            while (transactionDate < DateTime.Now)
            {
                Console.WriteLine($"handling date : {transactionDate}");
                var allTransactionUntilDate = transactions.Where(x => x.CreatedOn <= transactionDate).ToList();
                var allAccountbalancesTillDate = accountbalances.Where(x => x.CreatedOn <= transactionDate).ToList();
                //var totalInvestedForDate = allTransactionUntilDate.Where(x => x.TransactionType != TransactionType.SELL).Sum(x => x.TotalCosts) - allTransactionUntilDate.Where(x => x.TransactionType == TransactionType.SELL).Sum(x => x.TotalCosts);

                var totalInvestedForDate = allAccountbalancesTillDate.Sum(x => x.Value);

                double totalActualOfAllAssetsValue = 0;

                foreach (var transaction in allTransactionUntilDate.GroupBy(x => x.AssetId))
                {
                    Console.WriteLine($"Handling asset {transaction.Key}");
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
                
                var totalSpend = _dbContext.AccountBalance.Where(x => x.DepositType == DepositType.DEPOSIT && x.BrokerType == BrokerType.DEGIRO && x.UserID == userId && x.CreatedOn <= transactionDate).Sum(x => x.Value)
                    - _dbContext.AccountBalance.Where(x => x.DepositType == DepositType.WITHDRAW && x.BrokerType == BrokerType.DEGIRO && x.UserID == userId && x.CreatedOn <= transactionDate).Sum(x => x.Value);
                var totalTransactions = _dbContext.Transactions.Where(x=> x.UserID==userId && x.BrokerType == BrokerType.DEGIRO && x.TransactionType == TransactionType.BUY && x.CreatedOn <= transactionDate).Sum(x => x.TotalCosts) - _dbContext.Transactions.Where(x => x.UserID == userId && x.BrokerType == BrokerType.DEGIRO && x.TransactionType == TransactionType.SELL && x.CreatedOn <= transactionDate).Sum(x => x.TotalCosts);

                var cashAsset = _dbContext.Assets.FirstOrDefault(x => x.AssetType == AssetType.Cash);


                var portfolioHistory = new PortfolioHistory();
                portfolioHistory.UserID = userId;
                portfolioHistory.Date = transactionDate;
                portfolioHistory.TotalInvestedPortfolioValue = totalInvestedForDate;
                portfolioHistory.TotalPortfolioValue = Convert.ToDecimal(totalActualOfAllAssetsValue) + totalSpend -totalTransactions;
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
