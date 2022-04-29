using PortfolioTracker.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioTracker.Implementation.Services
{
    public class PortfolioService : IPortfolioService
    {
        private readonly MPortfolioDBContext _dbContext;

        public PortfolioService(MPortfolioDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task UpdatePortfolio()
        {
            var transactions = _dbContext.Transactions.ToList();
            var transactionGroupByAsset = transactions.GroupBy(x => x.AssetId);
            decimal totalValue = 0;
            foreach (var item in transactionGroupByAsset)
            {
                var portfolio = _dbContext.Portfolio.FirstOrDefault(x => x.AssetID == item.Key);
                bool isUpdate = false;
                if (portfolio != null)
                {
                    isUpdate = true;
                }
                else
                {
                    portfolio = new Portfolio();
                }

                portfolio.TotalShares = item.Where(x=>x.TransactionType!= TransactionType.SELL).Sum(x => x.AmountOfShares) - item.Where(x => x.TransactionType == TransactionType.SELL).Sum(x => x.AmountOfShares);
                
                if(portfolio.TotalShares == 0)
                {
                    portfolio.TotalInvestedValue = item.Where(x => x.TransactionType != TransactionType.SELL).Sum(x => x.TotalCosts);
                    portfolio.TotalValue = item.Where(x => x.TransactionType == TransactionType.SELL).Sum(x => x.TotalCosts);
                }
                else
                {
                    portfolio.TotalInvestedValue = item.Where(x => x.TransactionType != TransactionType.SELL).Sum(x => x.TotalCosts) - item.Where(x => x.TransactionType == TransactionType.SELL).Sum(x => x.TotalCosts);
                    if (_dbContext.Assets.Single(x => x.AssetId == item.Key).AssetType != AssetType.Groepsverzekering)
                    {
                        portfolio.TotalValue = _dbContext.Assets.Single(x => x.AssetId == item.Key).Value * portfolio.TotalShares;
                    }
                    else
                    {
                        portfolio.TotalValue = portfolio.TotalInvestedValue;
                    }
                }
                var divider = portfolio.TotalShares == 0 ? 1: portfolio.TotalShares;
                portfolio.AveragePricePerShare = portfolio.TotalInvestedValue / divider;

                portfolio.AssetID = item.Key;
                portfolio.ProfitPercentage = (portfolio.TotalValue - portfolio.TotalInvestedValue) / portfolio.TotalInvestedValue * 100;
                portfolio.Profit = portfolio.TotalValue - portfolio.TotalInvestedValue;

                if (isUpdate)
                {
                    _dbContext.Portfolio.Update(portfolio);
                }
                else
                {
                    await _dbContext.Portfolio.AddAsync(portfolio);
                }
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
