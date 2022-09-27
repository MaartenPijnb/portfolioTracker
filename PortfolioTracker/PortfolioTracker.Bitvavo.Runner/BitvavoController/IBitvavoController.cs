using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioTracker.Bitvavo.Runner.BitvavoController
{
    public interface IBitvavoController
    {
        Task ImportBitvavo(StreamReader bitvavoCsvStream, long userId);
    }
}
