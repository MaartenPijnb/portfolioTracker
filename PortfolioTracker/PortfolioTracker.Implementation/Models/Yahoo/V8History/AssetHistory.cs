using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioTracker.Implementation.Models.Yahoo.V8History
{
    public class AssetHistory
    {
        public string Symbol { get; set; }

        public List<int> Timestamp { get; set; }

        public List<double?> Close { get; set; }

        public object PreviousClose { get; set; }

        public double ChartPreviousClose { get; set; }

        public object Start { get; set; }

        public object End { get; set; }

        public int DataGranularity { get; set; }
    }
}
