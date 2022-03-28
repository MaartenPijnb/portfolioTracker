using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioTracker.Model
{
    public class API
    {
        public int APIId { get; set; }
        public APIType APIName { get; set; }
        public string BaseUrl { get; set; }
        public string APIKey { get; set; }
    }

    public enum APIType
    {
        YAHOOFINANCE = 0,
        BINANCE =1,
        NOTAPPLICABLE = 3,
    }
}
