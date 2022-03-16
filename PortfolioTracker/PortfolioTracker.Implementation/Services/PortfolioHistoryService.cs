using PortfolioTracker.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioTracker.Implementation.Services
{
    public class PortfolioHistoryService : IPortfolioHistoryService
    {
        private readonly MPortfolioDBContext _dbContext;

        public PortfolioHistoryService(MPortfolioDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task CreatePortfolioHistory()
        {
            var portfolios = _dbContext.Portfolio.ToList();
            var portfolioHistory = new PortfolioHistory
            {
                TotalInvestedPortfolioValue = portfolios.Sum(x => x.TotalInvestedValue),
                TotalPortfolioValue = portfolios.Sum(x => x.TotalValue)
            };
            portfolioHistory.Profit = portfolioHistory.TotalPortfolioValue - portfolioHistory.TotalInvestedPortfolioValue;
            portfolioHistory.Percentage = (portfolioHistory.TotalPortfolioValue - portfolioHistory.TotalInvestedPortfolioValue) / portfolioHistory.TotalInvestedPortfolioValue * 100;
            await _dbContext.PortfolioHistory.AddAsync(portfolioHistory);
            await _dbContext.SaveChangesAsync();
        }
    }
}
