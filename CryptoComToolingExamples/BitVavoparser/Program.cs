// See https://aka.ms/new-console-template for more information
using BitVavoparser.CSVEntities;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

Console.WriteLine("Hello, World!");


Console.WriteLine("Loading CSV....");


//var test = "Mon Jan 03 2022 13:04:20 GMT+0000 (Coordinated Universal Time)";

//var leuk = DateTime.ParseExact(test, "ddd MMM dd yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

//Console.WriteLine(leuk);



using (var reader = new StreamReader("C:\\Users\\PaijnzzzPC\\source\\repos\\CryptoComTooling\\CryptoComTooling\\transactions_bitvavo.csv"))
using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
{
    int TotalRecords = 0;
    csv.Context.RegisterClassMap<BitvavoRecordMap>();
    var cryptoRecords = csv.GetRecords<BitvavoRecord>().ToList();
    Console.WriteLine($"{cryptoRecords.Count} aantal records gevonden");

    Console.WriteLine($"----- Geld uitgegeven in Bitvavo -----");
    double depositTotal = 0;
    foreach (var depositRecord in cryptoRecords.Where(x=>x.Type==BitvavoType.deposit))
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

    List<PortfolioTransactionRecord> portfolioTransactionRecords = new List<PortfolioTransactionRecord>();  

    foreach (var tradedDistinctByDateRecord in tradedDistinctByDateRecords)
    {
        Console.WriteLine("hello" + tradedDistinctByDateRecord.First().TimeStamp + tradedDistinctByDateRecord.First().Currency);
       

        //first record is always in euro and determines if its a buy or sell
        var firstTradedRecord = tradedDistinctByDateRecord.First();

        if(firstTradedRecord.Currency != "EUR")
        {
            throw new Exception("Eerste record van de transactie was geen euro, we konden niet berekenen of het over een buy of sell order ging.");
        }

        BitVavoparser.CSVEntities.Action transactionType;
        if (firstTradedRecord.Amount < 0)
        {
            transactionType = BitVavoparser.CSVEntities.Action.Bought;
        }
        else
        {
            transactionType = BitVavoparser.CSVEntities.Action.Sold;
        }

        Console.WriteLine("We hebben een " + transactionType + " aankoop gehad.");

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
            if(tradedRecord.Currency == "EUR")
            {
                totalEur += tradedRecord.Amount;
            }
            else
            {
                totalCryptoCurrency += tradedRecord.Amount;
            }

        }
        if (transactionType == BitVavoparser.CSVEntities.Action.Sold)
        {
            totalCryptoCurrency = totalCryptoCurrency * -1;
        }
        else
        {
            totalEur = totalEur * -1;   
        }
        portfolioTransactionRecords.Add( new PortfolioTransactionRecord
        {
            Date = tradedDistinctByDateRecord.Key.Date,
            Action = transactionType,
            Symbol = cryptoCurrencySymbol,
            Name =  $"{cryptoCurrencySymbol}",
            Shares = totalCryptoCurrency,
            Price = (1/ totalCryptoCurrency) * totalEur,   

        });
        
    }




    foreach (var tradedRecord in cryptoRecords.Where(x => x.Type == BitvavoType.trade))
    {
        //Console.WriteLine($"Op {tradedRecord.TimeStamp} heb je {tradedRecord.Amount} {tradedRecord.Currency} uitgegeven");
    }

    Console.WriteLine("--------------------");
    var coinnamesAndStakedValue = new Dictionary<string, double>();

    foreach (var stakingRecord in cryptoRecords.Where(x => x.Type == BitvavoType.staking))
    {
        if (coinnamesAndStakedValue.ContainsKey(stakingRecord.Currency))
        {
            coinnamesAndStakedValue[stakingRecord.Currency] += stakingRecord.Amount; 
        }
        else
        {
            coinnamesAndStakedValue.Add(stakingRecord.Currency, stakingRecord.Amount);
        }
        //Console.WriteLine($"Op {stakingRecord.TimeStamp} heb je {stakingRecord.Amount} {stakingRecord.Currency} gekregen");     

    }

    foreach (var coinnameAndStakedValue in coinnamesAndStakedValue)
    {
        portfolioTransactionRecords.Add(new PortfolioTransactionRecord
        {
            Date = DateTime.Now.Date,
            Action = BitVavoparser.CSVEntities.Action.Bought,
            Symbol = coinnameAndStakedValue.Key,
            Name = coinnameAndStakedValue.Key,
            Shares = coinnameAndStakedValue.Value,
            Price = 0 //staking is free :)
        });
    }

    portfolioTransactionRecords = portfolioTransactionRecords.OrderBy(x => x.Date).ToList(); 

    foreach (var portfolioRecord in portfolioTransactionRecords)
    {
        Console.WriteLine($"Op {portfolioRecord.Date} heb je een {portfolioRecord.Action} gedaan van {portfolioRecord.Shares} {portfolioRecord.Symbol} aan {portfolioRecord.Price} euro");
    }


    using (var writer = new StreamWriter("C:\\Users\\PaijnzzzPC\\source\\repos\\CryptoComTooling\\CryptoComTooling\\transactions_bitvavo_generated_small_stake.csv"))
    using (var csvWriter = new CsvWriter(writer, new CsvConfiguration(CultureInfo.CurrentCulture) { Delimiter = ";" }))
    {
        csvWriter.WriteRecords(portfolioTransactionRecords);
    }

}





Console.ReadLine();
    