// See https://aka.ms/new-console-template for more information
using System.Text.Json;
using System.Net.Http.Json;
using PortfolioTracker.Model;

Console.WriteLine("Testing Yahoo Finance API");

var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.Add("x-api-key", "6PoQCJV9O17jeUPS81UDN1sHJ86gKB4RahYraKSS");

var serializeOptions = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true

};
var quoteResponse = await httpClient.GetFromJsonAsync<Root>("https://yfapi.net/v6/finance/quote?region=US&lang=en&symbols=IWDA.AS,IEMA.AS", serializeOptions);
var dbContext = new MPortfolioDBContext(new Microsoft.EntityFrameworkCore.DbContextOptions<MPortfolioDBContext>());

if (quoteResponse != null)
{
    foreach (var item in quoteResponse.QuoteResponse.Result)
    {
        Console.WriteLine($"ETF : {item.LongName} heeft als waarde {item.RegularMarketPrice}");
        var assetToUpdate = dbContext.Assets.Single(x => x.SymbolForApi == item.Symbol);
        assetToUpdate.Value = (decimal)item.RegularMarketPrice;
        assetToUpdate.UpdatedOn = DateTime.Now;
        dbContext.Update(assetToUpdate);
    }
}
await dbContext.SaveChangesAsync();

//put this to playground

var transactions = dbContext.Transactions.ToList();
var transactionGroupByAsset = transactions.GroupBy(x => x.AssetId);
decimal totalValue = 0;
foreach (var item in transactionGroupByAsset)
{
    var portfolio = dbContext.Portfolio.FirstOrDefault(x => x.AssetID == item.Key);
    bool isUpdate = false;
    if (portfolio != null)
    {
        isUpdate = true;
    }
    else
    {
        portfolio = new Portfolio();
    }

    portfolio.TotalShares = item.Sum(x => x.AmountOfShares);
    portfolio.TotalInvestedValue = item.Sum(x => x.TotalCosts);
    portfolio.AveragePricePerShare = item.Average(x => x.PricePerShare);
    portfolio.TotalValue = dbContext.Assets.Single(x=>x.AssetId== item.Key).Value * portfolio.TotalShares;
    portfolio.AssetID = item.Key;
    portfolio.ProfitPercentage =  (portfolio.TotalValue - portfolio.TotalInvestedValue) / portfolio.TotalInvestedValue  * 100;

    if (isUpdate)
    {
        dbContext.Portfolio.Update(portfolio);
    }
    else
    {
        await dbContext.Portfolio.AddAsync(portfolio);
    }
}

await dbContext.SaveChangesAsync();

//create portfolio history
var portfolios = dbContext.Portfolio.ToList();
var portfolioHistory = new PortfolioHistory
{
    TotalInvestedPortfolioValue = portfolios.Sum(x => x.TotalInvestedValue),
    TotalPortfolioValue = portfolios.Sum(x => x.TotalValue)
};
portfolioHistory.Profit = portfolioHistory.TotalPortfolioValue - portfolioHistory.TotalInvestedPortfolioValue;
portfolioHistory.Percentage = (portfolioHistory.TotalPortfolioValue - portfolioHistory.TotalInvestedPortfolioValue) / portfolioHistory.TotalInvestedPortfolioValue * 100;
await dbContext.PortfolioHistory.AddAsync(portfolioHistory);
await dbContext.SaveChangesAsync();

Console.ReadLine();