namespace PortfolioTracker.Model
{
    
    public class PortfolioTransaction
    {
        public int PortfolioTransactionId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Symbol { get; set; }
    }
}