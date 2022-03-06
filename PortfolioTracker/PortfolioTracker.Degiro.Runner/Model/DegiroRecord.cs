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
    internal class DegiroRecord
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

        public class DegiroRecordMap : ClassMap<DegiroRecord>
        {
            public DegiroRecordMap()
            {
                Map(m => m.CreatedOnDate).TypeConverter<BitvavoDateConverter<DateTime>>().Name("Datum");
                Map(m => m.CreatedOnTime).Name("Tijd");
                Map(m => m.Description).Name("Omschrijving");
                Map(m => m.ValutaDate).TypeConverter<BitvavoDateConverter<DateTime>>().Name("Valutadatum");
                Map(m => m.ProductName).Name("Product");
                Map(m => m.Currency).Name("Mutatie");
                Map(m => m.TotalPrice).Index(8);
                Map(m => m.CurrencySaldo).Name("Saldo");
                Map(m => m.RemainingValue).Index(10);
                Map(m => m.OrderID);
                Map(m => m.ISIN);
                Map(m => m.FX);

            }
        }


        public class BitvavoDateConverter<T> : DefaultTypeConverter
        {
            public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
            {
                return DateTime.ParseExact(text, "d-M-yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }

            public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
            {
                DateTime datetime = (DateTime)value;
                return datetime.ToString();
            }
        }

    }
}
