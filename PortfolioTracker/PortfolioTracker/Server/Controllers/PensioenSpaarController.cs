using Microsoft.AspNetCore.Mvc;
using PortfolioTracker.Implementation.Resolvers;
using PortfolioTracker.Model;

namespace PortfolioTracker.Server.Controllers
{
    public class PensioenSpaarController : Controller
    {
        private readonly MPortfolioDBContext _dbContext;
        private readonly IAssetValueResolver _assetvalueResolver;

        public PensioenSpaarController(MPortfolioDBContext dbContext, IAssetValueResolver assetValueResolver)
        {
            _dbContext = dbContext;
            _assetvalueResolver = assetValueResolver;
        }

        [Route("CreatePensioenSpaarTransaction")]
        [HttpPost]
        public async Task<IActionResult> CreatePensioenSpaarTransaction(int assetId, decimal totalShares, decimal totalValue, decimal percentage)
        {
            //46,705
            var startedOn = new DateTime(2017, 8, 17);
            var totalMonths = (DateTime.Now.Year - startedOn.Year) * 12 + DateTime.Now.Month - startedOn.Month;


            //Caculate original price per share 
            var totalPercentage = 100 + percentage;
            var valueForOnePercent = totalValue / totalPercentage;

            var transactionDate = startedOn;
            var transactionsToAdd = new List<PortfolioTransaction>();

            while (transactionDate < DateTime.Now)
            {
                var transaction = new PortfolioTransaction();
                transaction.AssetId = assetId;
                transaction.AmountOfShares = totalShares / totalMonths;
                transaction.TotalCosts = valueForOnePercent * 100 / totalMonths;
                transaction.PricePerShare = transaction.TotalCosts / transaction.AmountOfShares;
                transaction.TransactionType = TransactionType.BUY;
                transaction.CreatedOn = transactionDate;

                transactionsToAdd.Add(transaction);
                transactionDate = transactionDate.AddMonths(1);
            }


            await _dbContext.Transactions.AddRangeAsync(transactionsToAdd);

            await _dbContext.SaveChangesAsync();

            return Ok();
        }


        [Route("CreatePensioenSpaarTransactionAutomaticaly")]
        [HttpPost]
        public async Task<IActionResult> CreatePensioenSpaarTransactionv2()
        {
            //Started in 
            var argentaAsset = _dbContext.Assets.First(x => x.AssetType == AssetType.Pensioen && x.Name.Contains("Argenta"));
            var currentPrice = await _assetvalueResolver.GetAssetValue(argentaAsset.API.APIName, new List<string> { argentaAsset.SymbolForApi });

            var transaction = new PortfolioTransaction();
            transaction.AssetId = argentaAsset.AssetId;
            transaction.PricePerShare = Convert.ToDecimal(currentPrice[0]);
            transaction.TotalCosts = 82.50m; // DIFFERS EACH YEAR, HARDCODED 2022 VALUE IS 990 EURO PER MONTH
            transaction.AmountOfShares = transaction.TotalCosts / transaction.PricePerShare;
            transaction.TransactionType = TransactionType.BUY;
            transaction.CreatedOn = DateTime.Now;

            await _dbContext.Transactions.AddAsync(transaction);

            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [Route("CreateGroepsVerzekeringTransactionAutomaticaly")]
        [HttpPost]
        public async Task<IActionResult> CreateGroepsVerzekeringTransactionAutomaticaly()
        {
            //Started in 
            var groepsVerzekeringAsset = _dbContext.Assets.First(x => x.AssetType == AssetType.Groepsverzekering && x.Name.Contains("IS"));

            var transaction = new PortfolioTransaction();
            transaction.AssetId = groepsVerzekeringAsset.AssetId;
            transaction.PricePerShare = 178.13m;
            transaction.TotalCosts = 178.13m; 
            transaction.AmountOfShares = transaction.TotalCosts / transaction.PricePerShare;
            transaction.TransactionType = TransactionType.BUY;
            transaction.CreatedOn = DateTime.Now;

            await _dbContext.Transactions.AddAsync(transaction);

            await _dbContext.SaveChangesAsync();

            return Ok();
        }


        [Route("CreateGroepsverzekeringTransaction")]
        [HttpPost]
        public async Task<IActionResult> CreateGroepsverzekeringTransaction(int assetId, DateTime startedOn)
        {
            var totalMonths = (DateTime.Now.Year - startedOn.Year) * 12 + DateTime.Now.Month - startedOn.Month;
            var transactionDate = startedOn;
            var transactionsToAdd = new List<PortfolioTransaction>();

            while (transactionDate < DateTime.Now)
            {
                var transaction = new PortfolioTransaction();
                transaction.AssetId = assetId;
                transaction.AmountOfShares = 1;
                transaction.TotalCosts = 8022.33M / totalMonths;
                transaction.PricePerShare = 8022.33M / totalMonths;
                transaction.TransactionType = TransactionType.BUY;
                transaction.CreatedOn = transactionDate;

                transactionsToAdd.Add(transaction);
                transactionDate = transactionDate.AddMonths(1);
            }


            await _dbContext.Transactions.AddRangeAsync(transactionsToAdd);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
