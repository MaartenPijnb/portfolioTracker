using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitVavoparser.CSVEntities
{
    internal class PortfolioTransactionRecord
    {
        public DateTime Date { get; set; }
        public Action Action { get; set; }
        public string Symbol { get; set; }
        public string Sector { get; } = "Cryptocurrencies";
        public string Type { get; } = "Cryptocurrency";
        public string Name { get; set; }    
        public string FillerRecordG { get; set; }
        public string FillerRecordH { get; set; }
        public double Shares { get; set; }
        public double Price { get; set; }
        public double Commission { get; } = 0;
        public double TOB { get; } = 0;
        public double TotalTax { get; } = 0;
    }

    public enum Action
    {
        Unknown,
        Bought,
        Sold
    }
}
