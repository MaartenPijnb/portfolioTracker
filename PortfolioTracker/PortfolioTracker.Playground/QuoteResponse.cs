public class Result
{
    public string Language { get; set; }
    public string Region { get; set; }
    public string QuoteType { get; set; }
    public string TypeDisp { get; set; }
    public bool Triggerable { get; set; }
    public string CustomPriceAlertConfidence { get; set; }
    public string Currency { get; set; }
    public double RegularMarketPreviousClose { get; set; }
    public double Bid { get; set; }
    public double Ask { get; set; }
    public int BidSize { get; set; }
    public int AskSize { get; set; }
    public string FullExchangeName { get; set; }
    public double RegularMarketOpen { get; set; }
    public int AverageDailyVolume3Month { get; set; }
    public int AverageDailyVolume10Day { get; set; }
    public double FiftyTwoWeekLowChange { get; set; }
    public double FiftyTwoWeekLowChangePercent { get; set; }
    public string FiftyTwoWeekRange { get; set; }
    public double FiftyTwoWeekHighChange { get; set; }
    public double FiftyTwoWeekHighChangePercent { get; set; }
    public double FiftyTwoWeekLow { get; set; }
    public double FiftyTwoWeekHigh { get; set; }
    public string MarketState { get; set; }
    public object FirstTradeDateMilliseconds { get; set; }
    public int PriceHint { get; set; }
    public double RegularMarketChange { get; set; }
    public double RegularMarketChangePercent { get; set; }
    public int RegularMarketTime { get; set; }
    public double RegularMarketPrice { get; set; }
    public double RegularMarketDayHigh { get; set; }
    public string RegularMarketDayRange { get; set; }
    public double RegularMarketDayLow { get; set; }
    public int RegularMarketVolume { get; set; }
    public double FiftyDayAverage { get; set; }
    public double FiftyDayAverageChange { get; set; }
    public double FiftyDayAverageChangePercent { get; set; }
    public double TwoHundredDayAverage { get; set; }
    public double TwoHundredDayAverageChange { get; set; }
    public double TwoHundredDayAverageChangePercent { get; set; }
    public int SourceInterval { get; set; }
    public int ExchangeDataDelayedBy { get; set; }
    public bool Tradeable { get; set; }
    public string Exchange { get; set; }
    public string ShortName { get; set; }
    public string LongName { get; set; }
    public string MessageBoardId { get; set; }
    public string ExchangeTimezoneName { get; set; }
    public string ExchangeTimezoneShortName { get; set; }
    public int GmtOffSetMilliseconds { get; set; }
    public string Market { get; set; }
    public bool EsgPopulated { get; set; }
    public string Symbol { get; set; }
}

public class QuoteResponse
{
    public List<Result> Result { get; set; }
    public object Error { get; set; }
}

public class Root
{
    public QuoteResponse QuoteResponse { get; set; }
}

