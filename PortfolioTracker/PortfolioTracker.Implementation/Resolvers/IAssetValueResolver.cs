using PortfolioTracker.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioTracker.Implementation.Resolvers
{
    public interface IAssetValueResolver
    {
        Task<List<double>> GetAssetValue(APIType apiType, List<string> symbols);
    }
}
