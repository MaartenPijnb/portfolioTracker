using PortfolioTracker.Model.Common;

namespace PortfolioTracker.Model
{
    
    public class PortfolioTransaction
    {
        public int PortfolioTransactionId { get; set; }
        public DateTime CreatedOn { get; set; }
        public CurrencyType CurencyType { get; set; }
        public TransactionType TransactionType{ get; set; }
        public decimal AmountOfShares{ get; set; }
        public decimal PricePerShare { get; set; }
        public decimal TotalCosts { get; set; }
        public decimal TransactionCosts { get; set; }
        public decimal TaxesCosts { get; set; }
        public Guid? OrderId { get; set; }
        public int AssetId{ get; set; }
        public Asset Asset{ get; set; }        
        public BrokerType BrokerType{ get; set; }

    }

    public enum TransactionType
    {
        BUY,
        SELL,
        STAKING,
        CREDITCARD_CASHBACK,
        REFERAL
    }

    public enum BrokerType
    {
        UNKNOWN,
        DEGIRO,
        BITVAVO,
        CRYPTOCOM,
        CRYTPCOMEXCHANGE,
        HOTBIT
    }
}