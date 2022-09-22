﻿using Microsoft.AspNetCore.Mvc;
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

        [Route("UpdateAllAssets")]
        [HttpPost]
        public async Task UpdateAllAssets()
        {
            await _assetService.UpdateAssets();
        }
    }
}