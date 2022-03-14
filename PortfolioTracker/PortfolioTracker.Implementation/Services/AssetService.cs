using PortfolioTracker.Implementation.APIs;
using PortfolioTracker.Implementation.Resolvers;
using PortfolioTracker.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioTracker.Implementation.Services
{
    public class AssetService : IAssetService
    {
        private readonly MPortfolioDBContext _dbcontext;
        private readonly IAssetValueResolver _assetValueResolver;

        public AssetService(MPortfolioDBContext dbContext, IAssetValueResolver assetValueResolver)
        {
            _dbcontext = dbContext;
            _assetValueResolver = assetValueResolver;
        }
        public async Task UpdateAssets()
        {
            var allAssets = _dbcontext.Assets.ToList();

            foreach (var assetsPerApi in allAssets.GroupBy(x=>x.API.APIName))
            {
                var currentValueOfAsset = await _assetValueResolver.GetAssetValue(assetsPerApi.Key, assetsPerApi.Select(x =>x.SymbolForApi).ToList());
                int element = 0; 
                foreach (var asset in assetsPerApi)
                {
                    asset.UpdatedOn = DateTime.Now;
                    asset.Value = Convert.ToDecimal(currentValueOfAsset[element]);
                    _dbcontext.Update(asset);
                    element++;
                }               
            }

            await _dbcontext.SaveChangesAsync();
        }
    }
}
