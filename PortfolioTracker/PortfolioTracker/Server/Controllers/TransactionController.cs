using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioTracker.Implementation.Resolvers;
using PortfolioTracker.Implementation.Services;
using PortfolioTracker.Model;

namespace PortfolioTracker.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransactionController : Controller
    {
        private readonly MPortfolioDBContext _dbContext;
        
        public TransactionController( MPortfolioDBContext dBContext)
        {
            _dbContext = dBContext;
        }


        [HttpGet]
        [Route("TotalTransactionCosts/{UserId}")]
        public async Task<decimal> GetTotalTransactionCosts(long UserId) => await _dbContext.Transactions.Where(x=>x.UserID==UserId).SumAsync(x => x.TransactionCosts);
        

        [HttpGet]
        [Route("TotalTaxes/{UserId}")]
        public async Task<decimal> GetTotalTaxes(long UserId) => await _dbContext.Transactions.Where(x => x.UserID == UserId).SumAsync(x => x.TaxesCosts);

    }
}
