using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using CsvHelper.TypeConversion;
using PortfolioTracker.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioTracker.Degiro.Runner.Model
{
    public class DegiroRecord
    {           
        public DateTime CreatedOnDate{ get; set; }
        public TimeOnly CreatedOnTime { get; set; }
        public DateTime ValutaDate{ get; set; }
        public string? ProductName { get; set; }
        public string? ISIN { get; set; }
        public string Description{ get; set; }
        public string? FX { get; set; }
        public CurrencyType? Currency{ get; set; }
        public decimal? TotalPrice{ get; set; }
        public CurrencyType? CurrencySaldo { get; set; }
        public decimal? RemainingValue { get; set; }
        public Guid? OrderID { get; set; }
    }
}
