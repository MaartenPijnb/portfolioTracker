using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioTracker.Implementation.Services;
using PortfolioTracker.Model;

namespace PortfolioTracker.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssetsController : Controller
    {
        private readonly MPortfolioDBContext _dbContext;
        private readonly IAssetService _assetService;

        public AssetsController(MPortfolioDBContext dbContext, IAssetService assetService)
        {
            _dbContext = dbContext;
            _assetService = assetService;
        }


        [HttpGet]
        public async Task<List<Asset>> GetAll() => await _dbContext.Assets.ToListAsync();

        [HttpGet]
        [Route("{assetId}")]
        public async Task<Asset> Get(int assetId) => await _dbContext?.Assets?.SingleOrDefaultAsync(asset => asset.AssetId == assetId);

        [HttpPost]
        public async Task UpdateAsset([FromBody] Asset asset)
        {
            var assetFromDb = await _dbContext?.Assets?.SingleOrDefaultAsync(bla => bla.AssetId == asset.AssetId);

            assetFromDb.Name = asset.Name;
            assetFromDb.ISN = asset.ISN;
            assetFromDb.SymbolForApi = asset.SymbolForApi;
            await _dbContext.SaveChangesAsync();
        }

        [Route("UpdateAllAssets")]
        [HttpPost]
        public async Task UpdateAllAssets()
        {
            await _assetService.UpdateAssets();
        }
    }
}
