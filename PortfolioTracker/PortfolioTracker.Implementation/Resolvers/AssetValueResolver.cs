using PortfolioTracker.Implementation.APIs;
using PortfolioTracker.Implementation.Models;
using PortfolioTracker.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioTracker.Implementation.Resolvers
{
    public class AssetValueResolver : IAssetValueResolver
    {
        private readonly IYahooFinanceClient _yahooFinanceClient;

        public AssetValueResolver(IYahooFinanceClient yahooFinanceClient)
        {
            _yahooFinanceClient = yahooFinanceClient;
        }
        public async Task<List<double>> GetAssetValue(APIType apiType, List<string> symbols)
        {
            switch (apiType)
            {
                case APIType.YAHOOFINANCE:
                    var result = await _yahooFinanceClient.GetYahooFinanceRootResultForSymbols(symbols);
                    return result.QuoteResponse.Result.Select(x => x.RegularMarketPrice).ToList();                    
                case APIType.BINANCE:
                    throw new NotImplementedException($"{apiType.ToString()} is not supported");
                default:
                case APIType.NOTAPPLICABLE:
                    return new List<double>{0 };
                    throw new NotSupportedException();
            }
        }

        //TODO: Properly refactor the model to support dynamic symbols and max 5 symbols
        public async Task<List<AssetHistory>> GetAssetValueHistory(APIType apiType, IEnumerable<string> symbols)
        {
            List<AssetHistory> response = new();
            switch (apiType)
            {
                case APIType.YAHOOFINANCE:
                    var assetHistoryResponse = await _yahooFinanceClient.GetAssetHistoryRespone(symbols);

                    foreach (var assetHistory in assetHistoryResponse)
                    {
                        var newAssetHistory = new AssetHistory();
                        newAssetHistory.SymbolName = assetHistory.Key;
                        for (int i = 0; i < assetHistory.Value.Timestamp.Count; i++)
                        {
                            var datetimeoffset = DateTimeOffset.FromUnixTimeSeconds(assetHistory.Value.Timestamp[i]);
                            if (assetHistory.Value.Close[i].HasValue)
                            {
                                newAssetHistory.AssetHistoryValues.Add(datetimeoffset.Date, assetHistory.Value.Close[i].Value);
                            }
                        }                        
                        response.Add(newAssetHistory);
                    }

                    return response;         

                case APIType.BINANCE:
                    throw new NotImplementedException($"{apiType.ToString()} is not supported");
                default:
                    throw new NotSupportedException();
            }
        }
    }
}

