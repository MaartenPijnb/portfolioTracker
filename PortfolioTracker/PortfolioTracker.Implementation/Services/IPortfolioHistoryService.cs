using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioTracker.Implementation.Services
{
    public interface IPortfolioHistoryService
    {
        Task CreatePortfolioHistory(long userId);
    }
}
