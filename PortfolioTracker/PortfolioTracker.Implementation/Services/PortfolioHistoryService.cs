using PortfolioTracker.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioTracker.Implementation.Services
{
    public class PortfolioHistoryService : IPortfolioHistoryService
    {
        private readonly MPortfolioDBContext _dbContext;

        public PortfolioHistoryService(MPortfolioDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Task CreatePortfolioHistory()
        {
            throw new NotImplementedException();
        }
    }
}
