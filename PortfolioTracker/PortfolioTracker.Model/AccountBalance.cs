using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioTracker.Model
{
    public class AccountBalance
    {
        public int AccountBalanceId { get; set; }
        public BrokerType BrokerType { get; set; }
        public decimal Value { get; set; }
        public DateTime CreatedOn { get; set; }
        public DepositType DepositType { get; set; }
    }

    public enum DepositType
    {
        DEPOSIT, WITHDRAW
    }
}
