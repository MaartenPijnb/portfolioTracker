// See https://aka.ms/new-console-template for more information
using CsvHelper;
using CsvHelper.Configuration;
using PortfolioTracker.Bitvavo.Runner.Model;
using PortfolioTracker.Model;
using System.Globalization;

Console.WriteLine("Hello, World!");


Console.WriteLine("Loading CSV....");

using (var reader = new StreamReader("C:\\Git\\portfolioTracker\\CryptoComToolingExamples\\BitVavoparser\\transactions_bitvavo.csv"))
using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
{
    int TotalRecords = 0;
    csv.Context.RegisterClassMap<BitvavoRecordMap>();
    var cryptoRecords = csv.GetRecords<BitvavoRecord>().ToList();
    Console.WriteLine($"{cryptoRecords.Count} aantal records gevonden");

    Console.WriteLine($"----- Geld uitgegeven in Bitvavo -----");
    double depositTotal = 0;
    foreach (var depositRecord in cryptoRecords.Where(x => x.Type == BitvavoType.deposit))
    {
        depositTotal += depositRecord.Amount;
        Console.WriteLine($"Op {depositRecord.TimeStamp} heb je {depositRecord.Amount} {depositRecord.Currency} uitgegeven");
    }
    Console.WriteLine($"Totaal: {depositTotal} euro");
    Console.WriteLine($"----- Geld uitgegeven in Bitvavo -----");

    var tradedRecords = cryptoRecords.Where(x => x.Type == BitvavoType.trade).ToList();
    Console.WriteLine($"Totale Traded records (incl. duplicates) {tradedRecords.Count}");

    var tradedDistinctByDateRecords = tradedRecords.GroupBy(x => x.TimeStamp);
    Console.WriteLine($"Totale Traded records (excl. duplicates) {tradedDistinctByDateRecords.Count()}");

    var portfolioTransactionRecords = new List<PortfolioTransaction>();
        
    foreach (var tradedDistinctByDateRecord in tradedDistinctByDateRecords)
    {
        //first record is always in euro and determines if its a buy or sell
        var firstTradedRecord = tradedDistinctByDateRecord.First();

        //if(firstTradedRecord.Currency != "EUR")
        //{
        //    throw new Exception("Eerste record van de transactie was geen euro, we konden niet berekenen of het over een buy of sell order ging.");
        //}

        TransactionType transactionType;
        if (firstTradedRecord.Amount < 0)
        {
            transactionType = TransactionType.BUY;
        }
        else
        {
            transactionType = TransactionType.SELL;
        }

        var lastRecord = tradedDistinctByDateRecord.Last();
        if (lastRecord.Currency == "EUR")
        {
            throw new Exception("Laatste record van de transactie mag geen euro, we konden niet berekenen of het over een buy of sell order ging.");
        }
        var cryptoCurrencySymbol = lastRecord.Currency;

        double totalEur = 0;
        double totalCryptoCurrency = 0;
        foreach (var tradedRecord in tradedDistinctByDateRecord)
        {
            if (tradedRecord.Currency == "EUR")
            {
                totalEur += tradedRecord.Amount;
            }
            else
            {
                totalCryptoCurrency += tradedRecord.Amount;
            }

        }
        if (transactionType == TransactionType.SELL)
        {
            totalCryptoCurrency = totalCryptoCurrency * -1;
        }
        else
        {
            totalEur = totalEur * -1;
        }
        //kosten worden berkend op basis van 0.25%, kan fout zijn omwille dat deze gegevens niet zijn aangeleverd in de .csv
        portfolioTransactionRecords.Add(new PortfolioTransaction
        {
            CreatedOn = tradedDistinctByDateRecord.Key.Date,
            TransactionType = transactionType,
            AmountOfShares = Convert.ToDecimal(totalCryptoCurrency),
            TransactionCosts = Convert.ToDecimal(totalEur) * 0.0025m,
            TotalCosts = Convert.ToDecimal(totalEur),    
            PricePerShare = Convert.ToDecimal(totalEur) / Convert.ToDecimal(totalCryptoCurrency),            
            //SymbolTEMP = cryptoCurrencySymbol
        });
    }


    Console.WriteLine("--------------------");
    var coinnamesAndStakedValue = new Dictionary<string, double>();

    foreach (var stakingRecord in cryptoRecords.Where(x => x.Type == BitvavoType.staking))
    {
        portfolioTransactionRecords.Add(new PortfolioTransaction
        {
            CreatedOn =stakingRecord.TimeStamp,
            TransactionType = TransactionType.STAKING,
            AmountOfShares = Convert.ToDecimal(stakingRecord.Amount),
            TransactionCosts = 0,
            TotalCosts = 0,
            PricePerShare = 0,
            //SymbolTEMP = stakingRecord.Currency
        });

        if (coinnamesAndStakedValue.ContainsKey(stakingRecord.Currency))
        {
            coinnamesAndStakedValue[stakingRecord.Currency] += stakingRecord.Amount;
        }
        else
        {
            coinnamesAndStakedValue.Add(stakingRecord.Currency, stakingRecord.Amount);
        }
    }

    portfolioTransactionRecords = portfolioTransactionRecords.OrderBy(x => x.CreatedOn).ToList();

    foreach (var portfolioRecord in portfolioTransactionRecords)
    {
        Console.WriteLine($"Op {portfolioRecord.CreatedOn} heb je een {portfolioRecord.TransactionType} gedaan van {portfolioRecord.AmountOfShares}  aan {portfolioRecord.TotalCosts} euro");
    }   

}





Console.ReadLine();
