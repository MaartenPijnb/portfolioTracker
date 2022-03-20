using PortfolioTracker.Implementation.Models.Yahoo;
using PortfolioTracker.Implementation.Models.Yahoo.V8History;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioTracker.Implementation.APIs
{
    public interface IYahooFinanceClient
    {
        Task<YahooFinanceRootResult> GetYahooFinanceRootResultForSymbols(List<string> symbols);
        Task<AssetHistoryResponse> GetAssetHistoryRespone(IEnumerable<string> symbols);
    }
}
