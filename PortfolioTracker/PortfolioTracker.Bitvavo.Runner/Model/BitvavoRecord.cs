using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using CsvHelper.TypeConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioTracker.Bitvavo.Runner.Model
{
    public class BitvavoRecord
    {
        [Name("timestamp")]
        public DateTime TimeStamp { get; set; }

        [Name("type")]
        public BitvavoType Type { get; set; }

        [Name("currency")]
        public string Currency { get; set; }

        [Name("amount")]
        public double Amount { get; set; }

        [Name("status")]
        public string Status { get; set; }
    }

    public enum BitvavoType
    {
        deposit,
        staking,
        trade,
        withdrawal
    }

    public class BitvavoRecordMap : ClassMap<BitvavoRecord>
    {
        public BitvavoRecordMap()
        {
            Map(m => m.TimeStamp).TypeConverter<BitvavoDateConverter<DateTime>>().Name("timestamp");
            Map(m => m.Type).Name("type");
            Map(m => m.Currency).Name("currency");
            Map(m => m.Amount).Name("amount");
            Map(m => m.Status).Name("status");
        }
    }


    public class BitvavoDateConverter<T> : DefaultTypeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            //string example  = "Mon Jan 03 2022 13:04:20 GMT+0000 (Coordinated Universal Time)";
            var datetimeWithoutGMT = text.Split(" GMT").First();

            return DateTime.ParseExact(datetimeWithoutGMT, "ddd MMM dd yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
        }

        public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            DateTime datetime = (DateTime)value;
            return datetime.ToString();
        }
    }
}
