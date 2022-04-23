using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioTracker.CryptoCom.Runner.CryptoController
{
    public interface ICryptoController
    {
        Task ImportCryptoCom(StreamReader cryptocomStream);
    }
}
