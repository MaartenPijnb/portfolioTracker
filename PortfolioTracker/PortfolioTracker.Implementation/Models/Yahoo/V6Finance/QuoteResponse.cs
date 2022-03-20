using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioTracker.Implementation.Models.Yahoo
{
    public class QuoteResponse
    {
        public List<Result> Result { get; set; }
        public object Error { get; set; }
    }
}
