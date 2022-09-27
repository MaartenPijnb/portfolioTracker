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
        private List<Asset> _assets;
        public CryptoController(MPortfolioDBContext dbContext)
        {
            _dbcontext = dbContext;
            _assets = dbContext.Assets.ToList();
        }

      
        public async Task ImportCryptoCom(StreamReader cryptocomStream, long userId)
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
                        AmountOfShares = cryptoRecordEarn.Amount,
                        TotalCosts = 0,
                        AssetId = assetId,
                        UserID= userId
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
                        AmountOfShares = cryptoRecordCryptoPurchase.ToAmount.Value,
                        PricePerShare = cryptoRecordCryptoPurchase.NativeAmount / cryptoRecordCryptoPurchase.ToAmount.Value,                        
                        AssetId = assetId,
                        UserID=userId
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
                        AmountOfShares = cryptoRecordCryptoPurchase.Amount * -1,
                        PricePerShare = cryptoRecordCryptoPurchase.NativeAmount / cryptoRecordCryptoPurchase.Amount * -1,
                        AssetId = assetId,
                        UserID=userId
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
                        AmountOfShares = cryptoRecordCryptoPurchase.Amount,
                        PricePerShare = cryptoRecordCryptoPurchase.NativeAmount / cryptoRecordCryptoPurchase.Amount,
                        AssetId = assetId,
                        UserID=userId
                    };
                    portfolioRecord.TotalCosts = portfolioRecord.AmountOfShares * portfolioRecord.PricePerShare;

                    PortfolioTransactions.Add(portfolioRecord);
                    var accountBalance = new AccountBalance
                    {
                        BrokerType = BrokerType.CRYPTOCOM,
                        CreatedOn = cryptoRecordCryptoPurchase.Timestamp,
                        DepositType = DepositType.DEPOSIT,
                        Value = cryptoRecordCryptoPurchase.NativeAmount,
                        UserID=userId
                    };
                    _dbcontext.AccountBalance.Add(accountBalance);
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
                        AmountOfShares = cryptoRecordCryptoPurchaseFiatWallet.ToAmount.Value,
                        PricePerShare = cryptoRecordCryptoPurchaseFiatWallet.NativeAmount / cryptoRecordCryptoPurchaseFiatWallet.ToAmount.Value,
                        AssetId = assetId,
                        UserID=userId
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
                        AmountOfShares = cryptoRecordCryptoPurchaseFiatWallet.Amount * -1,
                        PricePerShare = cryptoRecordCryptoPurchaseFiatWallet.NativeAmount / cryptoRecordCryptoPurchaseFiatWallet.Amount * -1,
                        AssetId = assetId,
                        UserID=userId
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
                        AmountOfShares = cryptoRecordCardCashBack.Amount,
                        PricePerShare = 0,
                        TotalCosts = 0,
                        AssetId = assetId,
                        UserID=userId
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
                        AmountOfShares = cryptoRecordReferal.Amount,
                        PricePerShare = 0,
                        TotalCosts = 0,
                        AssetId = assetId,
                        UserID=userId
                    });
                }

                await _dbcontext.Transactions.AddRangeAsync(PortfolioTransactions);
                await _dbcontext.SaveChangesAsync();
                foreach (var portfolioRecord in PortfolioTransactions)
                {
                    Console.WriteLine($"Op {portfolioRecord.CreatedOn} heb je een {portfolioRecord.TransactionType} gedaan van {portfolioRecord.AmountOfShares} {portfolioRecord.AssetId} aan {portfolioRecord.PricePerShare} euro voor in totaal aan {portfolioRecord.TotalCosts}");
                }


                Console.WriteLine("crypto.com generated");
            }
        }

        public async Task ImportCryptoComFiat(StreamReader cryptocomStream, long userId)
        {
            using (var csv = new CsvReader(cryptocomStream, CultureInfo.InvariantCulture))
            {
                int TotalRecords = 0;
                var cryptoRecords = csv.GetRecords<CryptoComData>().ToList();

                foreach (var sepaDeposit in cryptoRecords.Where(x=>x.TransactionKind == TransactionKind.viban_deposit))
                {
                    var accountBalance = new AccountBalance
                    {
                        BrokerType = BrokerType.CRYPTOCOM,
                        CreatedOn = sepaDeposit.Timestamp,
                        DepositType = DepositType.DEPOSIT,
                        Value = sepaDeposit.NativeAmount,
                        UserID=userId
                    };
                    _dbcontext.AccountBalance.Add(accountBalance);
                }

                foreach (var sepaDeposit in cryptoRecords.Where(x => x.TransactionKind == TransactionKind.viban_card_top_up))
                {
                    var accountBalance = new AccountBalance
                    {
                        BrokerType = BrokerType.CRYPTOCOM,
                        CreatedOn = sepaDeposit.Timestamp,
                        DepositType = DepositType.WITHDRAWNTOCARD,
                        Value = sepaDeposit.NativeAmount * -1,
                        UserID=userId
                    };
                    _dbcontext.AccountBalance.Add(accountBalance);
                }

                foreach (var sepaDeposit in cryptoRecords.Where(x => x.TransactionKind == TransactionKind.viban_withdrawal))
                {
                    var accountBalance = new AccountBalance
                    {
                        BrokerType = BrokerType.CRYPTOCOM,
                        CreatedOn = sepaDeposit.Timestamp,
                        DepositType = DepositType.WITHDRAW,
                        Value = sepaDeposit.NativeAmount,
                        UserID=userId
                    };
                    _dbcontext.AccountBalance.Add(accountBalance);
                }

                
            }


            await _dbcontext.SaveChangesAsync();


        }

        private async Task<int> GetOrCreateAsset(string currency)
        {
            var assetId = 0;
            if (_assets.Any(x => x.ISN == currency))
            {
                assetId = _assets.Single(x => x.ISN == currency).AssetId;
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
                _assets.Add(newAsset);
            }

            return assetId;
        }
    }
}
