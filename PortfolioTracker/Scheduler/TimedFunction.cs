using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using PortfolioTracker.Scheduler.API;

namespace PortfolioTracker.Scheduler
{
    public class TimedFunction
    {
        [FunctionName("CreatePortfolioHistory")]
        public async Task Run([TimerTrigger("0 18 * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var client = new PortfolioTrackerClient("https://portfoliotrackerpaijnzzz.azurewebsites.net/", new HttpClient());
            await client.CreatePortfolioHistoryAsync(); 
        }

        [FunctionName("CreatePensioenSpaarAndGroepsVerzekeringTransaction")]
        public async Task RunAndCreateTransactions([TimerTrigger("0 12 28 * *")] TimerInfo myTimer, ILogger log)
        {
            var client = new PortfolioTrackerClient("https://portfoliotrackerpaijnzzz.azurewebsites.net/", new HttpClient());
            await client.CreatePensioenSpaarTransactionAutomaticalyAsync();
            await client.CreateGroepsVerzekeringTransactionAutomaticalyAsync();
        }
    }
}
