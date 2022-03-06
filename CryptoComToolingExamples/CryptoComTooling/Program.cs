// See https://aka.ms/new-console-template for more information
using BitVavoparser.CSVEntities;
using CryptoComTooling;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

Console.WriteLine("Loading CSV....");


using (var reader = new StreamReader("C:\\Users\\PaijnzzzPC\\source\\repos\\CryptoComTooling\\CryptoComTooling\\cryto_Maarten_022022.csv"))
using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
{
    int TotalRecords = 0;
    var cryptoRecords = csv.GetRecords<CryptoComData>().ToList();

    var currencyWithTotalAmount = new Dictionary<string, decimal>();
    decimal totalMoney = 0;
    foreach (var cryptoRecordEarn in cryptoRecords.Where(x => x.TransactionKind == TransactionKind.crypto_earn_interest_paid || x.TransactionKind == TransactionKind.mco_stake_reward))
    {
        TotalRecords++;
        totalMoney += cryptoRecordEarn.NativeAmount;
        if (currencyWithTotalAmount.TryGetValue(cryptoRecordEarn.Currency, out var totalAmount))
        {
            totalAmount += cryptoRecordEarn.Amount;
            currencyWithTotalAmount[cryptoRecordEarn.Currency] = totalAmount;
        }
        else
        {
            currencyWithTotalAmount.Add(cryptoRecordEarn.Currency, cryptoRecordEarn.Amount);
        }
    }
    Console.WriteLine("----- CRYPTO EARNS -----");
    var startDate = cryptoRecords.Where(x => x.TransactionKind == TransactionKind.crypto_earn_interest_paid || x.TransactionKind == TransactionKind.mco_stake_reward).OrderBy(x => x.Timestamp).First();
    var enddate = cryptoRecords.Where(x => x.TransactionKind == TransactionKind.crypto_earn_interest_paid || x.TransactionKind == TransactionKind.mco_stake_reward).OrderBy(x => x.Timestamp).Last();
    
    var totaldays =  enddate.Timestamp.DayOfYear - startDate.Timestamp.DayOfYear;   
    
    Console.WriteLine($"Totaal knaken in eur verdient {totalMoney} EUR");
    Console.WriteLine($"Totaal knaken per dag eur verdient {totalMoney / totaldays} EUR");
    List<PortfolioTransactionRecord> portfolioTransactionRecords = new List<PortfolioTransactionRecord>();
   

    Console.WriteLine("----- GEKOCHT MET CREDIT CARD of usdc-----");
    decimal creditCardPurchasedTotal = 0;
    foreach (var cryptoRecordCryptoPurchase in cryptoRecords.Where(x => x.TransactionKind == TransactionKind.crypto_exchange) )
    {
        TotalRecords++;
        creditCardPurchasedTotal += cryptoRecordCryptoPurchase.NativeAmount;
        Console.WriteLine($"{cryptoRecordCryptoPurchase.NativeAmount} Euro gekocht aan {cryptoRecordCryptoPurchase.ToAmount} {cryptoRecordCryptoPurchase.ToCurrency} voor  {cryptoRecordCryptoPurchase.Currency} voor {cryptoRecordCryptoPurchase.NativeAmount /  cryptoRecordCryptoPurchase.ToAmount} op {cryptoRecordCryptoPurchase.Timestamp}");

        portfolioTransactionRecords.Add(new PortfolioTransactionRecord
        {
            Action = BitVavoparser.CSVEntities.Action.Bought,
            Date = cryptoRecordCryptoPurchase.Timestamp,
            Name = cryptoRecordCryptoPurchase.ToCurrency,
            Symbol = cryptoRecordCryptoPurchase.ToCurrency,
            Shares = cryptoRecordCryptoPurchase.ToAmount.Value,
            Price = cryptoRecordCryptoPurchase.NativeAmount / cryptoRecordCryptoPurchase.ToAmount.Value
        });

        //TODO FIX USDC aftrekken / verkopen

        portfolioTransactionRecords.Add(new PortfolioTransactionRecord
        {
            Action = BitVavoparser.CSVEntities.Action.Sold,
            Date = cryptoRecordCryptoPurchase.Timestamp,
            Name = cryptoRecordCryptoPurchase.Currency,
            Symbol = cryptoRecordCryptoPurchase.Currency,
            Shares = cryptoRecordCryptoPurchase.Amount * -1,
            Price = cryptoRecordCryptoPurchase.NativeAmount / cryptoRecordCryptoPurchase.Amount * -1
        });


    }

    foreach (var cryptoRecordCryptoPurchase in cryptoRecords.Where(x => x.TransactionKind == TransactionKind.crypto_purchase ))
    {
        TotalRecords++;
        creditCardPurchasedTotal += cryptoRecordCryptoPurchase.NativeAmount;
        Console.WriteLine($"{cryptoRecordCryptoPurchase.NativeAmount} Euro gekocht aan {cryptoRecordCryptoPurchase.ToAmount} {cryptoRecordCryptoPurchase.ToCurrency} voor  {cryptoRecordCryptoPurchase.Currency} voor {cryptoRecordCryptoPurchase.NativeAmount / cryptoRecordCryptoPurchase.ToAmount} op {cryptoRecordCryptoPurchase.Timestamp}");

        portfolioTransactionRecords.Add(new PortfolioTransactionRecord
        {
            Action = BitVavoparser.CSVEntities.Action.Bought,
            Date = cryptoRecordCryptoPurchase.Timestamp,
            Name = cryptoRecordCryptoPurchase.Currency,
            Symbol = cryptoRecordCryptoPurchase.Currency,
            Shares = cryptoRecordCryptoPurchase.Amount, 
            Price = cryptoRecordCryptoPurchase.NativeAmount / cryptoRecordCryptoPurchase.Amount
        });

    }

    foreach (var cryptoRecordToExchange in cryptoRecords.Where(x => x.TransactionKind == TransactionKind.crypto_to_exchange_transfer))
    {
        TotalRecords++;
        creditCardPurchasedTotal += cryptoRecordToExchange.NativeAmount;
        Console.WriteLine($"{cryptoRecordToExchange.NativeAmount} Euro gekocht aan {cryptoRecordToExchange.ToAmount} {cryptoRecordToExchange.ToCurrency} voor  {cryptoRecordToExchange.Currency} voor {cryptoRecordToExchange.NativeAmount / cryptoRecordToExchange.ToAmount} op {cryptoRecordToExchange.Timestamp}");

        if(cryptoRecordToExchange.Currency == "USDC")
        {
            portfolioTransactionRecords.Add(new PortfolioTransactionRecord
            {
                Action = BitVavoparser.CSVEntities.Action.Sold,
                Date = cryptoRecordToExchange.Timestamp,
                Name = cryptoRecordToExchange.Currency,
                Symbol = cryptoRecordToExchange.Currency,
                Shares = cryptoRecordToExchange.Amount *-1,
                Price = cryptoRecordToExchange.NativeAmount / cryptoRecordToExchange.Amount * -1
            });
        }
      

    }



    Console.WriteLine($"TOTAAL GEKOCHT CREDITCARD: {creditCardPurchasedTotal} EURO" );

    Console.WriteLine("----- GEKOCHT MET FIAT WALLET-----");
    decimal fiatWalletPurchasedTotal = 0;
    foreach (var cryptoRecordCryptoPurchaseFiatWallet in cryptoRecords.Where(x => x.TransactionKind == TransactionKind.viban_purchase))
    {
        TotalRecords++;
        fiatWalletPurchasedTotal += cryptoRecordCryptoPurchaseFiatWallet.Amount;
        Console.WriteLine($"{cryptoRecordCryptoPurchaseFiatWallet.Amount} Euro gekocht voor {cryptoRecordCryptoPurchaseFiatWallet.ToAmount} {cryptoRecordCryptoPurchaseFiatWallet.ToCurrency} aan {cryptoRecordCryptoPurchaseFiatWallet.Amount / cryptoRecordCryptoPurchaseFiatWallet.ToAmount} op {cryptoRecordCryptoPurchaseFiatWallet.Timestamp} ");

        portfolioTransactionRecords.Add(new PortfolioTransactionRecord
        {
            Action = BitVavoparser.CSVEntities.Action.Bought,
            Date = cryptoRecordCryptoPurchaseFiatWallet.Timestamp,
            Name = cryptoRecordCryptoPurchaseFiatWallet.ToCurrency,
            Symbol = cryptoRecordCryptoPurchaseFiatWallet.ToCurrency,
            Shares = cryptoRecordCryptoPurchaseFiatWallet.ToAmount.Value,
            Price = cryptoRecordCryptoPurchaseFiatWallet.NativeAmount / cryptoRecordCryptoPurchaseFiatWallet.ToAmount.Value
        });

    }
    fiatWalletPurchasedTotal = fiatWalletPurchasedTotal * -1;
    Console.WriteLine($"TOTAAL GEKOCHT FIAT WALLET: {fiatWalletPurchasedTotal} EURO");

    Console.WriteLine("----- CRYPTO TRADED -----");


    decimal currenciesTradedToEuroTotal = 0;
    foreach (var cryptoRecordCryptoPurchaseFiatWallet in cryptoRecords.Where(x => x.TransactionKind == TransactionKind.crypto_viban_exchange))
    {
        TotalRecords++;
        currenciesTradedToEuroTotal += cryptoRecordCryptoPurchaseFiatWallet.Amount;
        Console.WriteLine($"{cryptoRecordCryptoPurchaseFiatWallet.Amount} Euro verkocht voor {cryptoRecordCryptoPurchaseFiatWallet.ToAmount} {cryptoRecordCryptoPurchaseFiatWallet.Currency}");

        portfolioTransactionRecords.Add(new PortfolioTransactionRecord
        {
            Action = BitVavoparser.CSVEntities.Action.Sold,
            Date = cryptoRecordCryptoPurchaseFiatWallet.Timestamp,
            Name = cryptoRecordCryptoPurchaseFiatWallet.Currency,
            Symbol = cryptoRecordCryptoPurchaseFiatWallet.Currency,
            Shares = cryptoRecordCryptoPurchaseFiatWallet.Amount * -1,
            Price = cryptoRecordCryptoPurchaseFiatWallet.NativeAmount / cryptoRecordCryptoPurchaseFiatWallet.Amount * -1
        });
    }
    currenciesTradedToEuroTotal = currenciesTradedToEuroTotal * -1;




    Console.WriteLine($"----- TOTALE UITGAVE CRYPTO: {fiatWalletPurchasedTotal + creditCardPurchasedTotal - currenciesTradedToEuroTotal} EURO -----");
    Console.WriteLine($"----- TOTALE UITGAVE CRYPTO: {fiatWalletPurchasedTotal + creditCardPurchasedTotal } EURO -----");


    Console.WriteLine("----- CRYPTO SUPERCHARGED -----");


    decimal currenciesInEuroSuperCharged = 0;
    foreach (var cryptoRecordSupercharged in cryptoRecords.Where(x => x.TransactionKind == TransactionKind.supercharger_reward_to_app_credited))
    {
        TotalRecords++;
        currenciesInEuroSuperCharged += cryptoRecordSupercharged.Amount;
    }
    Console.WriteLine($" TOTAAL SUPERCHARGED : {currenciesInEuroSuperCharged} ");


    foreach (var currency in currencyWithTotalAmount)
    {
        Console.WriteLine($"TOTAAL GEKREGEN {currency.Value} {currency.Key} ");
        portfolioTransactionRecords.Add(new PortfolioTransactionRecord
        {
            Action = BitVavoparser.CSVEntities.Action.Bought,
            Date = DateTime.Now,
            Name = $"{currency.Key} Stake",
            Symbol = currency.Key,
            Shares = currency.Value,
            Price = 0
        });
    }

    decimal totalAmountCashBackCard = 0;
    foreach (var cryptoRecordCardCashBack in cryptoRecords.Where(x => x.TransactionKind == TransactionKind.referral_card_cashback || x.TransactionKind == TransactionKind.reimbursement))
    {
        TotalRecords++;
        totalAmountCashBackCard += cryptoRecordCardCashBack.Amount;
    }
    Console.WriteLine("----- CRYTPO CARD CASHBACK -----");
    Console.WriteLine($"Totaal verdient met Card Cashback {totalAmountCashBackCard} CRO");
    portfolioTransactionRecords.Add(new PortfolioTransactionRecord
    {
        Action = BitVavoparser.CSVEntities.Action.Bought,
        Date = DateTime.Now,
        Name = "CRO Credit Card",
        Symbol = "CRO",
        Shares = totalAmountCashBackCard,
        Price = 0
    });

    decimal totalAmountReferal = 0;
    foreach (var cryptoRecordCardCashBack in cryptoRecords.Where(x => x.TransactionKind == TransactionKind.referral_bonus || x.TransactionKind == TransactionKind.referral_gift))
    {
        TotalRecords++;
        totalAmountReferal += cryptoRecordCardCashBack.Amount;
    }

    Console.WriteLine("----- CRYTPO CARD CASHBACK -----");
    Console.WriteLine($"Totaal verdient met Referall en signup bonus {totalAmountReferal} CRO");
    portfolioTransactionRecords.Add(new PortfolioTransactionRecord
    {
        Action = BitVavoparser.CSVEntities.Action.Bought,
        Date = DateTime.Now,
        Name = "Referal & Signup bonus",
        Symbol = "CRO",
        Shares = totalAmountReferal,
        Price = 0
    });


    Console.WriteLine($"----- RECORDS PROCESSED : {TotalRecords} RECORDS TOTAL {cryptoRecords.Count} -----");
    Console.WriteLine();

    foreach (var portfolioRecord in portfolioTransactionRecords)
    {
        Console.WriteLine($"Op {portfolioRecord.Date} heb je een {portfolioRecord.Action} gedaan van {portfolioRecord.Shares} {portfolioRecord.Symbol} aan {portfolioRecord.Price} euro");
    }



    using (var writer = new StreamWriter("C:\\Users\\PaijnzzzPC\\source\\repos\\CryptoComTooling\\CryptoComTooling\\cryto_Maarten_022022_generated.csv"))
    using (var csvWriter = new CsvWriter(writer, new CsvConfiguration(CultureInfo.CurrentCulture) { Delimiter = ";" }))
    {
        csvWriter.WriteRecords(portfolioTransactionRecords);
    }
    Console.WriteLine("crypto.com generated");
}


Console.ReadLine();