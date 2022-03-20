using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioTracker.Implementation.Models
{
    public class AssetHistory
    {
        public string SymbolName { get; set; }

        public Dictionary<DateTime, double> AssetHistoryValues { get; set; } = new();


    }
}
