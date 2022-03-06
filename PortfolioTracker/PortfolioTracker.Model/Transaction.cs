using PortfolioTracker.Model.Common;

namespace PortfolioTracker.Model
{
    
    public class PortfolioTransaction
    {
        public int PortfolioTransactionId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Symbol { get; set; }
        public CurrencyType CurencyType { get; set; }

        public int AssetId{ get; set; }
        public Asset Asset{ get; set; }
    }
}