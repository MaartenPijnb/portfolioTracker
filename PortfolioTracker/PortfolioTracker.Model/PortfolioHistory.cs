using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioTracker.Model
{
    public class PortfolioHistory
    {
        public int PortfolioHistoryId { get; set; }
        public decimal TotalInvestedPortfolioValue { get; set; }
        public decimal TotalPortfolioValue { get; set; }
        public decimal Percentage{ get; set; }
        public decimal Profit { get; set; } 
        public DateTime Date { get; set; } = DateTime.Now;
        public long UserID { get; set; }

    }
}
