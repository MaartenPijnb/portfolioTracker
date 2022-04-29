using Microsoft.AspNetCore.Mvc;
using PortfolioTracker.Model;

namespace PortfolioTracker.Server.Controllers
{
    public class ManualController : Controller
    {
        private readonly MPortfolioDBContext _dbContext;
        public ManualController(MPortfolioDBContext dbContext)
        {
            _dbContext = dbContext;
        }


        [HttpPost]
        [Route("ImportHardcodedCryptoComExchange")]
        public async Task<IActionResult> ImportHardcodedCryptoComExchange()
        {
            var assets = _dbContext.Assets.ToList();
            List<PortfolioTransaction> transactions = new();

            // Luna klopt niet 100% maar is voor 0 amount of shares in totaal uit te komen.
            var lunaAsset = assets.Single(x => x.ISN == "LUNA");
            transactions.Add(new PortfolioTransaction
            {
                AssetId = lunaAsset.AssetId,
                CreatedOn = new DateTime(2022, 3, 7),
                AmountOfShares = 11.74625202m,
                BrokerType = BrokerType.CRYTPCOMEXCHANGE,
                PricePerShare = 73.50m,
                TaxesCosts = 0,
                TransactionCosts = 3.40m,
                TransactionType = TransactionType.SELL,
                TotalCosts = 859.931m
            });

            var usdcAsset = assets.Single(x => x.ISN == "USDC");

            transactions.Add(new PortfolioTransaction
            {
                AssetId = usdcAsset.AssetId,
                CreatedOn = new DateTime(2022, 3, 7),
                AmountOfShares = 933.6956521739m,
                BrokerType = BrokerType.CRYTPCOMEXCHANGE,
                PricePerShare = 0.92m,
                TaxesCosts = 0,
                TransactionCosts = 0m,
                TransactionType = TransactionType.BUY,
                TotalCosts = 859.931m
            });

           var croAsset = assets.Single(x => x.ISN == "CRO");
            transactions.Add(new PortfolioTransaction
            {
                AssetId = croAsset.AssetId,
                CreatedOn = new DateTime(2021, 11, 25),
                AmountOfShares = 653.594m,
                BrokerType = BrokerType.CRYTPCOMEXCHANGE,
                PricePerShare = 0.66555m,
                TaxesCosts = 0,
                TransactionCosts = 1m,
                TransactionType = TransactionType.BUY,
                TotalCosts = 436m
            });
            transactions.Add(new PortfolioTransaction
            {
                AssetId = usdcAsset.AssetId,
                CreatedOn = new DateTime(2021, 11, 25),
                AmountOfShares = 500m,
                BrokerType = BrokerType.CRYTPCOMEXCHANGE,
                PricePerShare = 0.87m,
                TaxesCosts = 0,
                TransactionCosts = 1m,
                TransactionType = TransactionType.SELL,
                TotalCosts = 436m
            });

            transactions.Add(new PortfolioTransaction
            {
                AssetId = croAsset.AssetId,
                CreatedOn = new DateTime(2021, 11, 29),
                AmountOfShares = 1364.24M,
                BrokerType = BrokerType.CRYTPCOMEXCHANGE,
                PricePerShare = 0.65252486m,
                TaxesCosts = 0,
                TransactionCosts = 3m,
                TransactionType = TransactionType.BUY,
                TotalCosts = 890.482516m
            });
            transactions.Add(new PortfolioTransaction
            {
                AssetId = usdcAsset.AssetId,
                CreatedOn = new DateTime(2021, 11, 29),
                AmountOfShares = 1000.222m,
                BrokerType = BrokerType.CRYTPCOMEXCHANGE,
                PricePerShare = 0.89m,
                TaxesCosts = 0,
                TransactionCosts = 3m,
                TransactionType = TransactionType.SELL,
                TotalCosts = 890.482516m
            });

            transactions.Add(new PortfolioTransaction
            {
                AssetId = croAsset.AssetId,
                CreatedOn = new DateTime(2021, 12, 4),
                AmountOfShares = 935.805M,
                BrokerType = BrokerType.CRYTPCOMEXCHANGE,
                PricePerShare = 0.4796m,
                TaxesCosts = 0,
                TransactionCosts = 1m,
                TransactionType = TransactionType.BUY,
                TotalCosts = 448.482516m
            });
            transactions.Add(new PortfolioTransaction
            {
                AssetId = usdcAsset.AssetId,
                CreatedOn = new DateTime(2021, 12, 4),
                AmountOfShares = 510.013m,
                BrokerType = BrokerType.CRYTPCOMEXCHANGE,
                PricePerShare = 0.88m,
                TaxesCosts = 0,
                TransactionCosts = 1m,
                TransactionType = TransactionType.SELL,
                TotalCosts = 448.482516m
            });

            await _dbContext.Transactions.AddRangeAsync(transactions);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        [Route("ImportHardcodedCryptoComHotBit")]
        public async Task<IActionResult> ImportHardcodedHotBit()
        {
            var assets = _dbContext.Assets.ToList();
            List<PortfolioTransaction> transactions = new();

            var xrpAsset = assets.Single(x => x.ISN == "XRP");
            //477.04 in totaal
            transactions.Add(new PortfolioTransaction
            {
                AssetId = xrpAsset.AssetId,
                CreatedOn = new DateTime(2021, 11, 7),
                AmountOfShares = 477.07067574m,
                BrokerType = BrokerType.HOTBIT,
                PricePerShare = 1.04835m,
                TaxesCosts = 0,
                TransactionCosts = 4m,
                TransactionType = TransactionType.SELL,
                TotalCosts=500
            });

            //var donkAsset = new Asset
            //{
            //    APIId = 2,
            //    ISN = "DONK",
            //    SymbolForApi = "DONK-USD",
            //    Name = "DONK",
            //    AssetType = AssetType.Crypto,
            //    Value = 0
            //};

            //await _dbContext.Assets.AddAsync(donkAsset);
            //await _dbContext.SaveChangesAsync();
            var donkAsset = assets.Single(x => x.ISN == "DONK");

            transactions.Add(new PortfolioTransaction
            {
                AssetId = donkAsset.AssetId,
                CreatedOn = new DateTime(2021, 11, 7),
                AmountOfShares = 15800m,
                BrokerType = BrokerType.HOTBIT,
                PricePerShare = 0.02258694m,
                TaxesCosts = 0,
                TransactionCosts = 1m,
                TransactionType = TransactionType.BUY,
                TotalCosts = 356.7m
            });

            transactions.Add(new PortfolioTransaction
            {
                AssetId = donkAsset.AssetId,
                CreatedOn = new DateTime(2021, 11, 8),
                AmountOfShares = 15800m,
                BrokerType = BrokerType.HOTBIT,
                PricePerShare = 0.2523m,
                TaxesCosts = 0,
                TransactionCosts = 1m,
                TransactionType = TransactionType.SELL,
                TotalCosts = 397.70m
            });

            //var sunnyAsset = new Asset { 
            //    APIId=1,
            //    SymbolForApi= "SUNNY-USD",
            //    AssetType=AssetType.Crypto,
            //    ISN="SUNNY",
            //    Name="SUNNY",
            //    Value=0
            //};
            //await _dbContext.Assets.AddAsync(sunnyAsset);
            //await _dbContext.SaveChangesAsync();
            var sunnyAsset = assets.Single(x => x.ISN == "SUNNY");

            transactions.Add(new PortfolioTransaction
            {
                AssetId = sunnyAsset.AssetId,
                CreatedOn = new DateTime(2021, 11, 9),
                AmountOfShares = 6000M,
                BrokerType = BrokerType.HOTBIT,
                PricePerShare = 0.025M,
                TaxesCosts = 0,
                TransactionCosts = 1m,
                TransactionType = TransactionType.BUY,
                TotalCosts = 150
            });

            var croasset = assets.Single(x => x.ISN == "CRO");
            transactions.Add(new PortfolioTransaction
            {
                AssetId = croasset.AssetId,
                CreatedOn = new DateTime(2021, 11, 28),
                AmountOfShares = 651m,
                BrokerType = BrokerType.HOTBIT,
                PricePerShare = 0.6497m,
                TaxesCosts = 0,
                TransactionCosts = 1m,
                TransactionType = TransactionType.BUY,
                TotalCosts = 422.75m
            });

            await _dbContext.Transactions.AddRangeAsync(transactions);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
