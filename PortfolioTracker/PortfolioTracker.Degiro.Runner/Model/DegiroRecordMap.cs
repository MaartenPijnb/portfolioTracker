using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioTracker.Degiro.Runner.Model
{
    public class DegiroRecordMap : ClassMap<DegiroRecord>
    {
        public DegiroRecordMap()
        {
            Map(m => m.CreatedOnDate).TypeConverter<DegiroDateConverter<DateTime>>().Name("Datum");
            Map(m => m.CreatedOnTime).Name("Tijd");
            Map(m => m.Description).Name("Omschrijving");
            Map(m => m.ValutaDate).TypeConverter<DegiroDateConverter<DateTime>>().Name("Valutadatum");
            Map(m => m.ProductName).Name("Product");
            Map(m => m.Currency).Name("Mutatie");
            Map(m => m.TotalPrice).Index(8);
            Map(m => m.CurrencySaldo).Name("Saldo");
            Map(m => m.RemainingValue).Index(10);
            Map(m => m.OrderID).Name("Order ID");
            Map(m => m.ISIN);
            Map(m => m.FX);
        }
    }
}
