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
            var transaction = new PortfolioTransaction();
            transaction.AssetId = assetId;
            transaction.AmountOfShares = totalShares;

            //Caculate original price per share 
            var totalPercentage = 100 + percentage;
            var valueForOnePercent = totalValue / totalPercentage;

            transaction.TotalCosts = valueForOnePercent*100;
            transaction.PricePerShare = transaction.TotalCosts / totalShares;
            transaction.TransactionType = TransactionType.BUY;

            await _dbContext.Transactions.AddAsync(transaction);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
