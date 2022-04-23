using CryptoComTooling;
using CsvHelper;
using PortfolioTracker.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioTracker.CryptoCom.Runner.CryptoController
{
    public class CryptoController : ICryptoController
    {
        private readonly MPortfolioDBContext _dbcontext;

        public CryptoController(MPortfolioDBContext dbContext)
        {
            _dbcontext = dbContext;
        }

      
        public async Task ImportCryptoCom(StreamReader cryptocomStream)
        {
            using (var csv = new CsvReader(cryptocomStream, CultureInfo.InvariantCulture))
            {
                int TotalRecords = 0;
                var cryptoRecords = csv.GetRecords<CryptoComData>().ToList();
                List<PortfolioTransaction> PortfolioTransactions = new List<PortfolioTransaction>();

                foreach (var cryptoRecordEarn in cryptoRecords.Where(x => x.TransactionKind == TransactionKind.crypto_earn_interest_paid || x.TransactionKind == TransactionKind.mco_stake_reward))
                {
                    int assetId = await GetOrCreateAsset(cryptoRecordEarn.Currency);

                    PortfolioTransactions.Add(new PortfolioTransaction
                    {
                        TransactionType = TransactionType.STAKING,
                        BrokerType = BrokerType.CRYPTOCOM,
                        CreatedOn = cryptoRecordEarn.Timestamp,
                        SymbolTemp = cryptoRecordEarn.Currency,
                        AmountOfShares = cryptoRecordEarn.Amount,
                        TotalCosts = 0,
                        AssetId = assetId
                    });
                }

                var startDate = cryptoRecords.Where(x => x.TransactionKind == TransactionKind.crypto_earn_interest_paid || x.TransactionKind == TransactionKind.mco_stake_reward).OrderBy(x => x.Timestamp).First();
                var enddate = cryptoRecords.Where(x => x.TransactionKind == TransactionKind.crypto_earn_interest_paid || x.TransactionKind == TransactionKind.mco_stake_reward).OrderBy(x => x.Timestamp).Last();

                var totaldays = enddate.Timestamp.DayOfYear - startDate.Timestamp.DayOfYear;

                decimal creditCardPurchasedTotal = 0;
                foreach (var cryptoRecordCryptoPurchase in cryptoRecords.Where(x => x.TransactionKind == TransactionKind.crypto_exchange))
                {
                    TotalRecords++;
                    creditCardPurchasedTotal += cryptoRecordCryptoPurchase.NativeAmount;
                    int assetId = await GetOrCreateAsset(cryptoRecordCryptoPurchase.ToCurrency);

                    var portfoliotransactionBuy = new PortfolioTransaction
                    {
                        TransactionType = TransactionType.BUY,
                        BrokerType = BrokerType.CRYPTOCOM,
                        CreatedOn = cryptoRecordCryptoPurchase.Timestamp,
                        SymbolTemp = cryptoRecordCryptoPurchase.ToCurrency,
                        AmountOfShares = cryptoRecordCryptoPurchase.ToAmount.Value,
                        PricePerShare = cryptoRecordCryptoPurchase.NativeAmount / cryptoRecordCryptoPurchase.ToAmount.Value,                        
                        AssetId = assetId
                    };
                    portfoliotransactionBuy.TotalCosts = portfoliotransactionBuy.AmountOfShares * portfoliotransactionBuy.PricePerShare;

                    PortfolioTransactions.Add(portfoliotransactionBuy);
                    assetId = await GetOrCreateAsset(cryptoRecordCryptoPurchase.Currency);
                    // USDC aftrekken / verkopen
                    var portfoliotransactionSell = new PortfolioTransaction
                    {
                        TransactionType = TransactionType.SELL,
                        BrokerType = BrokerType.CRYPTOCOM,
                        CreatedOn = cryptoRecordCryptoPurchase.Timestamp,
                        SymbolTemp = cryptoRecordCryptoPurchase.Currency,
                        AmountOfShares = cryptoRecordCryptoPurchase.Amount * -1,
                        PricePerShare = cryptoRecordCryptoPurchase.NativeAmount / cryptoRecordCryptoPurchase.Amount * -1,
                        AssetId = assetId
                    };
                    portfoliotransactionSell.TotalCosts = portfoliotransactionBuy.AmountOfShares * portfoliotransactionBuy.PricePerShare;


                    PortfolioTransactions.Add(portfoliotransactionSell);


                }

                foreach (var cryptoRecordCryptoPurchase in cryptoRecords.Where(x => x.TransactionKind == TransactionKind.crypto_purchase))
                {
                    TotalRecords++;
                    creditCardPurchasedTotal += cryptoRecordCryptoPurchase.NativeAmount;
                    int assetId = await GetOrCreateAsset(cryptoRecordCryptoPurchase.Currency);

                    var portfolioRecord = new PortfolioTransaction
                    {
                        TransactionType = TransactionType.BUY,
                        BrokerType = BrokerType.CRYPTOCOM,
                        CreatedOn = cryptoRecordCryptoPurchase.Timestamp,
                        SymbolTemp = cryptoRecordCryptoPurchase.Currency,
                        AmountOfShares = cryptoRecordCryptoPurchase.Amount,
                        PricePerShare = cryptoRecordCryptoPurchase.NativeAmount / cryptoRecordCryptoPurchase.Amount,
                        AssetId = assetId
                    };
                    portfolioRecord.TotalCosts = portfolioRecord.AmountOfShares * portfolioRecord.PricePerShare;

                    PortfolioTransactions.Add(portfolioRecord);

                }

                decimal fiatWalletPurchasedTotal = 0;
                foreach (var cryptoRecordCryptoPurchaseFiatWallet in cryptoRecords.Where(x => x.TransactionKind == TransactionKind.viban_purchase))
                {
                    TotalRecords++;
                    fiatWalletPurchasedTotal += cryptoRecordCryptoPurchaseFiatWallet.Amount;
                    int assetId = await GetOrCreateAsset(cryptoRecordCryptoPurchaseFiatWallet.ToCurrency);

                    var transaction = new PortfolioTransaction
                    {
                        TransactionType = TransactionType.BUY,
                        BrokerType = BrokerType.CRYPTOCOM,
                        CreatedOn = cryptoRecordCryptoPurchaseFiatWallet.Timestamp,
                        SymbolTemp = cryptoRecordCryptoPurchaseFiatWallet.ToCurrency,
                        AmountOfShares = cryptoRecordCryptoPurchaseFiatWallet.ToAmount.Value,
                        PricePerShare = cryptoRecordCryptoPurchaseFiatWallet.NativeAmount / cryptoRecordCryptoPurchaseFiatWallet.ToAmount.Value,
                        AssetId = assetId
                    };
                    transaction.TotalCosts = transaction.AmountOfShares * transaction.PricePerShare;

                    PortfolioTransactions.Add(transaction);

                }
                fiatWalletPurchasedTotal = fiatWalletPurchasedTotal * -1;



                decimal currenciesTradedToEuroTotal = 0;
                foreach (var cryptoRecordCryptoPurchaseFiatWallet in cryptoRecords.Where(x => x.TransactionKind == TransactionKind.crypto_viban_exchange))
                {
                    TotalRecords++;
                    currenciesTradedToEuroTotal += cryptoRecordCryptoPurchaseFiatWallet.Amount;
                    int assetId = await GetOrCreateAsset(cryptoRecordCryptoPurchaseFiatWallet.Currency);

                    var transaction = new PortfolioTransaction
                    {
                        TransactionType = TransactionType.SELL,
                        BrokerType = BrokerType.CRYPTOCOM,
                        CreatedOn = cryptoRecordCryptoPurchaseFiatWallet.Timestamp,
                        SymbolTemp = cryptoRecordCryptoPurchaseFiatWallet.Currency,
                        AmountOfShares = cryptoRecordCryptoPurchaseFiatWallet.Amount * -1,
                        PricePerShare = cryptoRecordCryptoPurchaseFiatWallet.NativeAmount / cryptoRecordCryptoPurchaseFiatWallet.Amount * -1,
                        AssetId = assetId
                    };
                    transaction.TotalCosts = transaction.AmountOfShares * transaction.PricePerShare;

                    PortfolioTransactions.Add(transaction);
                }
                currenciesTradedToEuroTotal = currenciesTradedToEuroTotal * -1;

                decimal totalAmountCashBackCard = 0;
                foreach (var cryptoRecordCardCashBack in cryptoRecords.Where(x => x.TransactionKind == TransactionKind.referral_card_cashback || x.TransactionKind == TransactionKind.reimbursement))
                {
                    int assetId = await GetOrCreateAsset(cryptoRecordCardCashBack.Currency);

                    PortfolioTransactions.Add(new PortfolioTransaction
                    {
                        TransactionType = TransactionType.CREDITCARD_CASHBACK,
                        BrokerType = BrokerType.CRYPTOCOM,
                        CreatedOn = cryptoRecordCardCashBack.Timestamp,
                        SymbolTemp = cryptoRecordCardCashBack.Currency,
                        AmountOfShares = cryptoRecordCardCashBack.Amount,
                        PricePerShare = 0,
                        TotalCosts = 0,
                        AssetId = assetId
                    });
                }



                foreach (var cryptoRecordReferal in cryptoRecords.Where(x => x.TransactionKind == TransactionKind.referral_bonus || x.TransactionKind == TransactionKind.referral_gift))
                {
                    int assetId = await GetOrCreateAsset(cryptoRecordReferal.Currency);

                    PortfolioTransactions.Add(new PortfolioTransaction
                    {
                        TransactionType = TransactionType.REFERAL,
                        BrokerType = BrokerType.CRYPTOCOM,
                        CreatedOn = cryptoRecordReferal.Timestamp,
                        SymbolTemp = cryptoRecordReferal.Currency,
                        AmountOfShares = cryptoRecordReferal.Amount,
                        PricePerShare = 0,
                        TotalCosts = 0,
                        AssetId = assetId
                    });
                }




                foreach (var portfolioRecord in PortfolioTransactions)
                {
                    Console.WriteLine($"Op {portfolioRecord.CreatedOn} heb je een {portfolioRecord.TransactionType} gedaan van {portfolioRecord.AmountOfShares} {portfolioRecord.SymbolTemp} aan {portfolioRecord.PricePerShare} euro voor in totaal aan {portfolioRecord.TotalCosts}");
                }

                Console.WriteLine("crypto.com generated");
            }
        }

        private async Task<int> GetOrCreateAsset(string currency)
        {
            var assetId = 0;
            if (_dbcontext.Assets.Any(x => x.ISN == currency))
            {
                assetId = _dbcontext.Assets.Single(x => x.ISN == currency).AssetId;
            }
            else
            {
                //create Asset
                var newAsset = new Asset
                {
                    APIId = 1,
                    ISN = currency,
                    Name = currency,
                    SymbolForApi = currency + "-EUR",
                    UpdatedOn = DateTime.Now,
                    AssetType = AssetType.Crypto,
                };
                await _dbcontext.AddAsync(newAsset);
                await _dbcontext.SaveChangesAsync();

                assetId = newAsset.AssetId;
            }

            return assetId;
        }
    }
}
