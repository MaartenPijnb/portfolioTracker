using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioTracker.Degiro.Runner.Controller
{
    public interface IDegiroController
    {
        Task ImportDegiro(StreamReader degiroCsvStream, long userId);
    }
}
