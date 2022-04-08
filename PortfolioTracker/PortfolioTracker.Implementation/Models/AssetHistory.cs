using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioTracker.Implementation.Models
{
    public class AssetHistory
    {
        public string SymbolName { get; set; }

        public Dictionary<DateTime, double> AssetHistoryValues { get; set; } = new();


    }

    public static class Extensions
    {
        public static double TryGetValueFromDate(this AssetHistory assetHistory,  DateTime dateToCheck)
        {
            double output = 0;

            int daysToAdd = -1;

            if(assetHistory.AssetHistoryValues.Keys.OrderBy(x=>x.Date).First() > dateToCheck)
            {
                daysToAdd = 1;
            }

            while (output == 0)
            {
                if (assetHistory.AssetHistoryValues.ContainsKey(dateToCheck))
                {
                    output = assetHistory.AssetHistoryValues[dateToCheck];
                }
                else
                {
                    dateToCheck = dateToCheck.AddDays(daysToAdd);
                }
            }
            return output;
        }
    }
}
