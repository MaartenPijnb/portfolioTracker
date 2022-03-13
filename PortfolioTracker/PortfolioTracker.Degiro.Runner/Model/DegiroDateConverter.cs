using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioTracker.Degiro.Runner.Model
{
    public class DegiroDateConverter<T> : DefaultTypeConverter
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
