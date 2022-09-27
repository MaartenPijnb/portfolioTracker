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
        public async Task CreatePortfolioHistory(long userId)
        {
            var portfolios = _dbContext.Portfolio.Where(x=>x.UserID==userId).ToList();
            var portfolioHistory = new PortfolioHistory
            {
                TotalInvestedPortfolioValue = portfolios.Where(x=>x.TotalShares!=0).Sum(x => x.TotalInvestedValue),
                TotalPortfolioValue = portfolios.Where(x => x.TotalShares != 0).Sum(x => x.TotalValue),
                UserID=userId
            };
            portfolioHistory.Profit = portfolioHistory.TotalPortfolioValue - portfolioHistory.TotalInvestedPortfolioValue;
            portfolioHistory.Percentage = (portfolioHistory.TotalPortfolioValue - portfolioHistory.TotalInvestedPortfolioValue) / portfolioHistory.TotalInvestedPortfolioValue * 100;
            await _dbContext.PortfolioHistory.AddAsync(portfolioHistory);
            await _dbContext.SaveChangesAsync();
        }
    }
}
