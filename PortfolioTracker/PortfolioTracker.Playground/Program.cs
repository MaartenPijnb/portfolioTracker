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

var portfolioHistory = new PortfolioHistory();
var transactions = dbContext.Transactions.ToList();
portfolioHistory.TotalInvestedPortfolioValue = transactions.Sum(y => y.TotalCosts);
var transactionGroupByAsset = transactions.GroupBy(x => x.AssetId);
decimal totalValue = 0;
foreach (var item in transactionGroupByAsset)
{
    decimal totalShares = 0;
    foreach (var portfolioItem in item)
    {
        totalShares += portfolioItem.AmountOfShares;
    }
    var asset = dbContext.Assets.Single(x => x.AssetId == item.Key);
    totalValue += asset.Value * totalShares;
}
portfolioHistory.TotalPortfolioValue = totalValue;
portfolioHistory.Profit = portfolioHistory.TotalPortfolioValue - portfolioHistory.TotalInvestedPortfolioValue;
portfolioHistory.Percentage = 100 - (portfolioHistory.TotalInvestedPortfolioValue / portfolioHistory.TotalPortfolioValue * 100);
await dbContext.PortfolioHistory.AddAsync(portfolioHistory);

await dbContext.SaveChangesAsync();
Console.ReadLine();