using PortfolioTracker.Implementation.Models.Yahoo;
using PortfolioTracker.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PortfolioTracker.Implementation.APIs
{
    public class YahooFinanceClient : IYahooFinanceClient
    {
        private readonly HttpClient _httpclient;
        private readonly MPortfolioDBContext _dbContext;
        private readonly JsonSerializerOptions _serializeOptions;

        public YahooFinanceClient(HttpClient httpClient, MPortfolioDBContext dBContext)
        {
            _httpclient = httpClient;
            _dbContext = dBContext;
            _serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };

            InitializeAPI();
        }
        public async Task<YahooFinanceRootResult> GetYahooFinanceRootResultForSymbols(List<string> symbols)
        {
            var symbolsAsString = string.Join(",", symbols );
            return await _httpclient.GetFromJsonAsync<YahooFinanceRootResult>(_httpclient.BaseAddress + "v6/finance/quote?region=US&lang=en&symbols=" + symbolsAsString, _serializeOptions);
        }

        private void InitializeAPI()
        {
            var api = _dbContext.APIs.Single(x => x.APIName == APIType.YAHOOFINANCE);
            
            _httpclient.BaseAddress = new Uri(api.BaseUrl);
            _httpclient.DefaultRequestHeaders.Add("x-api-key", api.APIKey);
        }
    }
}
