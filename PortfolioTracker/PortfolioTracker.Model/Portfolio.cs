using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioTracker.Model
{
    public class Portfolio
    {
        public int PortfolioId { get; set; }
        public int AssetID { get; set; }
        public Asset Asset { get; set; }
        public decimal TotalShares { get; set; }
        public decimal AveragePricePerShare{ get; set; }
        public decimal TotalInvestedValue { get; set; }
        public decimal TotalValue { get; set; }
        public decimal ProfitPercentage { get; set; }
        public decimal Profit { get; set; }
        public DateTime UpdatedOn { get; set; } = DateTime.Now;

    }
}
