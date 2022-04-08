using Microsoft.AspNetCore.Mvc;
using PortfolioTracker.Model;

namespace PortfolioTracker.Server.Controllers
{
    public class PensioenSpaarController : Controller
    {
        private readonly MPortfolioDBContext _dbContext;
        public PensioenSpaarController(MPortfolioDBContext dbContext)
        {
            _dbContext = dbContext;
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


        [Route("CreateGroepsverzekeringTransaction")]
        [HttpPost]
        public async Task<IActionResult> CreateGroepsverzekeringTransaction(int assetId, DateTime startedOn)
        {
            var totalMonths = (DateTime.Now.Year - startedOn.Year) * 12 + DateTime.Now.Month - startedOn.Month;
            var transactionDate = startedOn;
            var transactionsToAdd = new List<PortfolioTransaction>();

            while(transactionDate < DateTime.Now)
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
