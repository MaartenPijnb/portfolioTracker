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
        [Route("TotalTransactionCosts")]
        public async Task<decimal> GetTotalTransactionCosts() => await _dbContext.Transactions.SumAsync(x => x.TransactionCosts);
        

        [HttpGet]
        [Route("TotalTaxes")]
        public async Task<decimal> GetTotalTaxes() => await _dbContext.Transactions.SumAsync(x => x.TaxesCosts);

    }
}
