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

                portfolio.TotalShares = item.Where(x => x.TransactionType != TransactionType.SELL).Sum(x => x.AmountOfShares) - item.Where(x => x.TransactionType == TransactionType.SELL).Sum(x => x.AmountOfShares);

                if (portfolio.TotalShares == 0)
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
                var divider = portfolio.TotalShares == 0 ? 1 : portfolio.TotalShares;
                portfolio.AveragePricePerShare = portfolio.TotalInvestedValue / divider;

                portfolio.AssetID = item.Key;
                if (portfolio.TotalInvestedValue != 0) {
                    portfolio.ProfitPercentage = (portfolio.TotalValue - portfolio.TotalInvestedValue) / portfolio.TotalInvestedValue * 100;
                }

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

            var degiroTotalSpend = _dbContext.AccountBalance.Where(x => x.DepositType == DepositType.DEPOSIT && x.BrokerType == BrokerType.DEGIRO).Sum(x => x.Value) - _dbContext.AccountBalance.Where(x => x.DepositType == DepositType.WITHDRAW && x.BrokerType == BrokerType.DEGIRO).Sum(x => x.Value);
            var degiroTotalTransactions = transactions.Where(x => x.BrokerType == BrokerType.DEGIRO && x.TransactionType == TransactionType.BUY).Sum(x => x.TotalCosts) - transactions.Where(x => x.BrokerType == BrokerType.DEGIRO && x.TransactionType == TransactionType.SELL).Sum(x => x.TotalCosts);

            var cashAsset = _dbContext.Assets.FirstOrDefault(x => x.AssetType == AssetType.Cash);


            var newCashAsset = new Asset();
            if (cashAsset == null)
            {
                newCashAsset = new Asset
                {
                    APIId = 2,
                    AssetType = AssetType.Cash,
                    ISN = "Cash",
                    Name = "Cash",
                    SymbolForApi = "Not Applicable",
                    Value = 0
                };
                _dbContext.Assets.Add(newCashAsset);
                await _dbContext.SaveChangesAsync();

                
            }

            var cashPortfolio = _dbContext.Portfolio.FirstOrDefault(x => x.Asset.AssetType == AssetType.Cash);
            if (cashPortfolio == null)
            {
                cashPortfolio = new Portfolio
                {
                    AveragePricePerShare = degiroTotalSpend - degiroTotalTransactions,
                    TotalValue = degiroTotalSpend - degiroTotalTransactions,
                    TotalInvestedValue = degiroTotalSpend - degiroTotalTransactions,
                    Profit= 0,
                    ProfitPercentage = 0,
                    TotalShares =1,
                    AssetID = cashAsset.AssetId
                };
                _dbContext.Portfolio.Add(cashPortfolio);
            }
            else
            {
                cashPortfolio.TotalValue = degiroTotalSpend - degiroTotalTransactions;                
                cashPortfolio.AveragePricePerShare = degiroTotalSpend - degiroTotalTransactions;
                cashPortfolio.TotalInvestedValue = degiroTotalSpend - degiroTotalTransactions;

                cashPortfolio.Profit = 0;

                _dbContext.Portfolio.Update(cashPortfolio);
            }
            await _dbContext.SaveChangesAsync();
        }
    }
}
