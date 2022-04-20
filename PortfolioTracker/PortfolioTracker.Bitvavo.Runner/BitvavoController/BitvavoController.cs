using CsvHelper;
using PortfolioTracker.Bitvavo.Runner.Model;
using PortfolioTracker.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioTracker.Bitvavo.Runner.BitvavoController
{
    public class BitvavoController : IBitvavoController
    {
        private readonly MPortfolioDBContext _dbcontext;

        public BitvavoController(MPortfolioDBContext dbContext)
        {
            _dbcontext = dbContext;
        }

        public async Task ImportBitvavo(StreamReader bitvavoCsvStream)
        {
            var allApis = _dbcontext.APIs.ToList();
            using (var csv = new CsvReader(bitvavoCsvStream, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<BitvavoRecordMap>();
                var cryptoRecords = csv.GetRecords<BitvavoRecord>().ToList();

                var tradedRecords = cryptoRecords.Where(x => x.Type == BitvavoType.trade).ToList();
                var tradedDistinctByDateRecords = tradedRecords.GroupBy(x => x.TimeStamp);
                var portfolioTransactionRecords = new List<PortfolioTransaction>();

                foreach (var tradedDistinctByDateRecord in tradedDistinctByDateRecords)
                {
                    var firstTradedRecord = tradedDistinctByDateRecord.First();

                    TransactionType transactionType;
                    if (firstTradedRecord.Amount < 0)
                    {
                        transactionType = TransactionType.BUY;
                    }
                    else
                    {
                        transactionType = TransactionType.SELL;
                    }

                    var lastRecord = tradedDistinctByDateRecord.Last();
                    if (lastRecord.Currency == "EUR")
                    {
                        throw new Exception("Laatste record van de transactie mag geen euro, we konden niet berekenen of het over een buy of sell order ging.");
                    }
                    var cryptoCurrencySymbol = lastRecord.Currency;

                    double totalEur = 0;
                    double totalCryptoCurrency = 0;
                    foreach (var tradedRecord in tradedDistinctByDateRecord)
                    {
                        if (tradedRecord.Currency == "EUR")
                        {
                            totalEur += tradedRecord.Amount;
                        }
                        else
                        {
                            totalCryptoCurrency += tradedRecord.Amount;
                        }

                    }
                    if (transactionType == TransactionType.SELL)
                    {
                        totalCryptoCurrency = totalCryptoCurrency * -1;
                    }
                    else
                    {
                        totalEur = totalEur * -1;
                    }

                    var assetId = 0;
                    if (_dbcontext.Assets.Any(x => x.ISN == cryptoCurrencySymbol))
                    {
                        assetId = _dbcontext.Assets.Single(x => x.ISN == cryptoCurrencySymbol).AssetId;
                    }
                    else
                    {
                        //create Asset
                        var newAsset = new Asset
                        {
                            APIId = 1,
                            ISN = cryptoCurrencySymbol,
                            Name = cryptoCurrencySymbol,
                            SymbolForApi = cryptoCurrencySymbol + "-EUR",
                            UpdatedOn = DateTime.Now,
                            AssetType = AssetType.Crypto,
                        };
                        await _dbcontext.AddAsync(newAsset);
                        await _dbcontext.SaveChangesAsync();

                        assetId = newAsset.AssetId;
                    }

                    //kosten worden berkend op basis van 0.25%, kan fout zijn omwille dat deze gegevens niet zijn aangeleverd in de .csv
                    portfolioTransactionRecords.Add(new PortfolioTransaction
                    {
                        CreatedOn = tradedDistinctByDateRecord.Key.Date,
                        TransactionType = transactionType,
                        AmountOfShares = Convert.ToDecimal(totalCryptoCurrency),
                        TransactionCosts = Convert.ToDecimal(totalEur) * 0.0025m,
                        TotalCosts = Convert.ToDecimal(totalEur),
                        PricePerShare = Convert.ToDecimal(totalEur) / Convert.ToDecimal(totalCryptoCurrency),
                        AssetId = assetId
                    });
                }
                var coinnamesAndStakedValue = new Dictionary<string, double>();

                foreach (var stakingRecord in cryptoRecords.Where(x => x.Type == BitvavoType.staking))
                {
                    var assetId = 0;
                    if (_dbcontext.Assets.Any(x => x.ISN == stakingRecord.Currency))
                    {
                        assetId = _dbcontext.Assets.Single(x => x.ISN == stakingRecord.Currency).AssetId;
                    }
                    else
                    {
                        //create Asset
                        var newAsset = new Asset
                        {
                            APIId = 1,
                            ISN = stakingRecord.Currency,
                            Name = stakingRecord.Currency,
                            SymbolForApi = stakingRecord.Currency + "-EUR",
                            UpdatedOn = DateTime.Now,
                            AssetType = AssetType.Crypto,
                        };
                        await _dbcontext.AddAsync(newAsset);
                        await _dbcontext.SaveChangesAsync();

                        assetId = newAsset.AssetId;
                    }
                    portfolioTransactionRecords.Add(new PortfolioTransaction
                    {
                        CreatedOn = stakingRecord.TimeStamp,
                        TransactionType = TransactionType.STAKING,
                        AmountOfShares = Convert.ToDecimal(stakingRecord.Amount),
                        TransactionCosts = 0,
                        TotalCosts = 0,
                        PricePerShare = 0,
                        AssetId = assetId
                    });

                    if (coinnamesAndStakedValue.ContainsKey(stakingRecord.Currency))
                    {
                        coinnamesAndStakedValue[stakingRecord.Currency] += stakingRecord.Amount;
                    }
                    else
                    {
                        coinnamesAndStakedValue.Add(stakingRecord.Currency, stakingRecord.Amount);
                    }
                }

                portfolioTransactionRecords = portfolioTransactionRecords.OrderBy(x => x.CreatedOn).ToList();
                await _dbcontext.Transactions.AddRangeAsync(portfolioTransactionRecords);
                await _dbcontext.SaveChangesAsync();
            }
        }
    }
}
