using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PortfolioTracker.Implementation.Models.Yahoo.V8History
{
    public class AssetHistoryResponse
    {
        [JsonPropertyName("IWDA.AS")]

        public AssetHistory AssetHistoriesIWDA { get; set; }

        [JsonPropertyName("IEMA.AS")]
        public AssetHistory AssetHistoriesIEMA { get; set; }
    }
}
