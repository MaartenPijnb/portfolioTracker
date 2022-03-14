using PortfolioTracker.Implementation.APIs;
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
                    throw new NotSupportedException();
            }
        }
    }
}
