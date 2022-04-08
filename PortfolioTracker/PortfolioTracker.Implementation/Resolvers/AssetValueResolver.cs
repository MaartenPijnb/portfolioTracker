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
                    break;
                case APIType.BINANCE:
                    throw new NotImplementedException($"{apiType.ToString()} is not supported");
                default:
                case APIType.NOTAPPLICABLE:
                    return new List<double>{0 };
                    throw new NotSupportedException();
            }
        }

        //TODO: Properly refactor the model to support dynamic symbols.
        public async Task<List<AssetHistory>> GetAssetValueHistory(APIType apiType, IEnumerable<string> symbols)
        {
            List<AssetHistory> response = new();
            switch (apiType)
            {
                case APIType.YAHOOFINANCE:
                    var assetHistoryResponse = await _yahooFinanceClient.GetAssetHistoryRespone(symbols);

                    var assetHistoryIWDA = new AssetHistory();
                    assetHistoryIWDA.SymbolName = "IWDA.AS";
                    for (int i = 0; i < assetHistoryResponse.AssetHistoriesIWDA.Timestamp.Count; i++)
                    {
                        var datetimeoffset = DateTimeOffset.FromUnixTimeSeconds(assetHistoryResponse.AssetHistoriesIWDA.Timestamp[i]);
                        assetHistoryIWDA.AssetHistoryValues.Add(datetimeoffset.Date, assetHistoryResponse.AssetHistoriesIWDA.Close[i].Value);
                    }

                    var assetHistoryIEMA = new AssetHistory();
                    assetHistoryIEMA.SymbolName = "IEMA.AS";
                    for (int i = 0; i < assetHistoryResponse.AssetHistoriesIEMA.Timestamp.Count; i++)
                    {
                        var datetimeoffset = DateTimeOffset.FromUnixTimeSeconds(assetHistoryResponse.AssetHistoriesIEMA.Timestamp[i]);
                        assetHistoryIEMA.AssetHistoryValues.Add(datetimeoffset.Date, assetHistoryResponse.AssetHistoriesIEMA.Close[i].Value);
                    }

                    var assetHistoryArgenta = new AssetHistory();
                    assetHistoryArgenta.SymbolName = "0P00000NFB.F";
                    for (int i = 0; i < assetHistoryResponse.AssetHistoriesArgenta.Timestamp.Count; i++)
                    {
                        var datetimeoffset = DateTimeOffset.FromUnixTimeSeconds(assetHistoryResponse.AssetHistoriesArgenta.Timestamp[i]);
                        if (assetHistoryResponse.AssetHistoriesArgenta.Close[i].HasValue)
                        {
                            assetHistoryArgenta.AssetHistoryValues.Add(datetimeoffset.Date, assetHistoryResponse.AssetHistoriesArgenta.Close[i].Value);
                        }
                    }

                    response.Add(assetHistoryIWDA);
                    response.Add(assetHistoryIEMA);
                    response.Add(assetHistoryArgenta);

                    return response;         

                case APIType.BINANCE:
                    throw new NotImplementedException($"{apiType.ToString()} is not supported");
                default:
                    throw new NotSupportedException();
            }
        }
    }
}

