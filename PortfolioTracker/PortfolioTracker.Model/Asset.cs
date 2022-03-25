using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioTracker.Model
{
    public class Asset
    {
        public int AssetId { get; set; }
        public string? ISN{ get; set; }
        public string Name { get; set; }
        public string SymbolForApi { get; set; }
        public decimal Value { get; set; }
        public DateTime UpdatedOn { get; set; }
        public int APIId { get; set; }
        public API API{ get; set; }
        public AssetType AssetType{ get; set; }
    }
}
