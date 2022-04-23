// See https://aka.ms/new-console-template for more information
using CryptoComTooling;
using CsvHelper;
using CsvHelper.Configuration;
using PortfolioTracker.Model;
using System.Globalization;

Console.WriteLine("Loading CSV....");


using (var reader = new StreamReader("C:\\Users\\PaijnzzzPC\\source\\repos\\portfolioTracker\\PortfolioTracker\\PortfolioTracker.CryptoCom.Runner\\cryptotransactions.csv"))
using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
{
    int TotalRecords = 0;
    var cryptoRecords = csv.GetRecords<CryptoComData>().ToList();
    List<PortfolioTransaction> PortfolioTransactions = new List<PortfolioTransaction>();

    var currencyWithTotalAmount = new Dictionary<string, decimal>();
    decimal totalMoney = 0;
    foreach (var cryptoRecordEarn in cryptoRecords.Where(x => x.TransactionKind == TransactionKind.crypto_earn_interest_paid || x.TransactionKind == TransactionKind.mco_stake_reward))
    {
        PortfolioTransactions.Add(new PortfolioTransaction
        {
            TransactionType = TransactionType.STAKING,
            BrokerType = BrokerType.CRYPTOCOM,
            CreatedOn = cryptoRecordEarn.Timestamp,            
            SymbolTemp = cryptoRecordEarn.Currency,
            AmountOfShares = cryptoRecordEarn.Amount,
            TotalCosts = 0
        });
    }
    Console.WriteLine("----- CRYPTO EARNS -----");
    var startDate = cryptoRecords.Where(x => x.TransactionKind == TransactionKind.crypto_earn_interest_paid || x.TransactionKind == TransactionKind.mco_stake_reward).OrderBy(x => x.Timestamp).First();
    var enddate = cryptoRecords.Where(x => x.TransactionKind == TransactionKind.crypto_earn_interest_paid || x.TransactionKind == TransactionKind.mco_stake_reward).OrderBy(x => x.Timestamp).Last();

    var totaldays = enddate.Timestamp.DayOfYear - startDate.Timestamp.DayOfYear;

    Console.WriteLine($"Totaal knaken in eur verdient {totalMoney} EUR");
    Console.WriteLine($"Totaal knaken per dag eur verdient {totalMoney / totaldays} EUR");


    Console.WriteLine("----- GEKOCHT MET CREDIT CARD of usdc-----");
    decimal creditCardPurchasedTotal = 0;
    foreach (var cryptoRecordCryptoPurchase in cryptoRecords.Where(x => x.TransactionKind == TransactionKind.crypto_exchange))
    {
        TotalRecords++;
        creditCardPurchasedTotal += cryptoRecordCryptoPurchase.NativeAmount;
        Console.WriteLine($"{cryptoRecordCryptoPurchase.NativeAmount} Euro gekocht aan {cryptoRecordCryptoPurchase.ToAmount} {cryptoRecordCryptoPurchase.ToCurrency} voor  {cryptoRecordCryptoPurchase.Currency} voor {cryptoRecordCryptoPurchase.NativeAmount / cryptoRecordCryptoPurchase.ToAmount} op {cryptoRecordCryptoPurchase.Timestamp}");

        var portfoliotransactionBuy = new PortfolioTransaction
        {
            TransactionType = TransactionType.BUY,
            BrokerType=BrokerType.CRYPTOCOM,
            CreatedOn = cryptoRecordCryptoPurchase.Timestamp,
            SymbolTemp = cryptoRecordCryptoPurchase.ToCurrency,
            AmountOfShares = cryptoRecordCryptoPurchase.ToAmount.Value,
            PricePerShare = cryptoRecordCryptoPurchase.NativeAmount / cryptoRecordCryptoPurchase.ToAmount.Value,
        };
        portfoliotransactionBuy.TotalCosts = portfoliotransactionBuy.AmountOfShares * portfoliotransactionBuy.PricePerShare;

        PortfolioTransactions.Add(portfoliotransactionBuy);

        // USDC aftrekken / verkopen
        var portfoliotransactionSell = new PortfolioTransaction
        {
            TransactionType = TransactionType.SELL,
            BrokerType = BrokerType.CRYPTOCOM,
            CreatedOn = cryptoRecordCryptoPurchase.Timestamp,
            SymbolTemp = cryptoRecordCryptoPurchase.Currency,
            AmountOfShares = cryptoRecordCryptoPurchase.Amount * -1,
            PricePerShare = cryptoRecordCryptoPurchase.NativeAmount / cryptoRecordCryptoPurchase.Amount * -1
        };
        portfoliotransactionSell.TotalCosts = portfoliotransactionBuy.AmountOfShares * portfoliotransactionBuy.PricePerShare;


        PortfolioTransactions.Add(portfoliotransactionSell);


    }

    foreach (var cryptoRecordCryptoPurchase in cryptoRecords.Where(x => x.TransactionKind == TransactionKind.crypto_purchase))
    {
        TotalRecords++;
        creditCardPurchasedTotal += cryptoRecordCryptoPurchase.NativeAmount;
        Console.WriteLine($"{cryptoRecordCryptoPurchase.NativeAmount} Euro gekocht aan {cryptoRecordCryptoPurchase.ToAmount} {cryptoRecordCryptoPurchase.ToCurrency} voor  {cryptoRecordCryptoPurchase.Currency} voor {cryptoRecordCryptoPurchase.NativeAmount / cryptoRecordCryptoPurchase.ToAmount} op {cryptoRecordCryptoPurchase.Timestamp}");

        var portfolioRecord = new PortfolioTransaction
        {
            TransactionType = TransactionType.BUY,
            BrokerType = BrokerType.CRYPTOCOM,
            CreatedOn = cryptoRecordCryptoPurchase.Timestamp,
            SymbolTemp = cryptoRecordCryptoPurchase.Currency,
            AmountOfShares = cryptoRecordCryptoPurchase.Amount,
            PricePerShare = cryptoRecordCryptoPurchase.NativeAmount / cryptoRecordCryptoPurchase.Amount
        };
        portfolioRecord.TotalCosts = portfolioRecord.AmountOfShares * portfolioRecord.PricePerShare;

        PortfolioTransactions.Add(portfolioRecord);

    }

    Console.WriteLine($"TOTAAL GEKOCHT CREDITCARD: {creditCardPurchasedTotal} EURO");

    Console.WriteLine("----- GEKOCHT MET FIAT WALLET-----");
    decimal fiatWalletPurchasedTotal = 0;
    foreach (var cryptoRecordCryptoPurchaseFiatWallet in cryptoRecords.Where(x => x.TransactionKind == TransactionKind.viban_purchase))
    {
        TotalRecords++;
        fiatWalletPurchasedTotal += cryptoRecordCryptoPurchaseFiatWallet.Amount;
        Console.WriteLine($"{cryptoRecordCryptoPurchaseFiatWallet.Amount} Euro gekocht voor {cryptoRecordCryptoPurchaseFiatWallet.ToAmount} {cryptoRecordCryptoPurchaseFiatWallet.ToCurrency} aan {cryptoRecordCryptoPurchaseFiatWallet.Amount / cryptoRecordCryptoPurchaseFiatWallet.ToAmount} op {cryptoRecordCryptoPurchaseFiatWallet.Timestamp} ");
        var transaction = new PortfolioTransaction
        {
            TransactionType = TransactionType.BUY,
            BrokerType = BrokerType.CRYPTOCOM,
            CreatedOn = cryptoRecordCryptoPurchaseFiatWallet.Timestamp,
            SymbolTemp = cryptoRecordCryptoPurchaseFiatWallet.ToCurrency,
            AmountOfShares = cryptoRecordCryptoPurchaseFiatWallet.ToAmount.Value,
            PricePerShare = cryptoRecordCryptoPurchaseFiatWallet.NativeAmount / cryptoRecordCryptoPurchaseFiatWallet.ToAmount.Value
        };
        transaction.TotalCosts = transaction.AmountOfShares * transaction.PricePerShare;

        PortfolioTransactions.Add(transaction);

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

        var transaction = new PortfolioTransaction
        {
            TransactionType = TransactionType.SELL,
            BrokerType = BrokerType.CRYPTOCOM,
            CreatedOn = cryptoRecordCryptoPurchaseFiatWallet.Timestamp,
            SymbolTemp = cryptoRecordCryptoPurchaseFiatWallet.Currency,
            AmountOfShares = cryptoRecordCryptoPurchaseFiatWallet.Amount * -1,
            PricePerShare = cryptoRecordCryptoPurchaseFiatWallet.NativeAmount / cryptoRecordCryptoPurchaseFiatWallet.Amount * -1
        };
        transaction.TotalCosts = transaction.AmountOfShares * transaction.PricePerShare;

        PortfolioTransactions.Add(transaction);
    }
    currenciesTradedToEuroTotal = currenciesTradedToEuroTotal * -1;




    Console.WriteLine($"----- TOTALE UITGAVE CRYPTO: {fiatWalletPurchasedTotal + creditCardPurchasedTotal - currenciesTradedToEuroTotal} EURO -----");
    Console.WriteLine($"----- TOTALE UITGAVE CRYPTO: {fiatWalletPurchasedTotal + creditCardPurchasedTotal } EURO -----");





    decimal totalAmountCashBackCard = 0;
    foreach (var cryptoRecordCardCashBack in cryptoRecords.Where(x => x.TransactionKind == TransactionKind.referral_card_cashback || x.TransactionKind == TransactionKind.reimbursement))
    {
        PortfolioTransactions.Add(new PortfolioTransaction
        {
            TransactionType = TransactionType.CREDITCARD_CASHBACK,
            BrokerType = BrokerType.CRYPTOCOM,
            CreatedOn = cryptoRecordCardCashBack.Timestamp,
                       SymbolTemp = cryptoRecordCardCashBack.Currency,
            AmountOfShares = cryptoRecordCardCashBack.Amount,
            PricePerShare = 0,
            TotalCosts=0
        });
    }


    
    foreach (var cryptoRecordReferal in cryptoRecords.Where(x => x.TransactionKind == TransactionKind.referral_bonus || x.TransactionKind == TransactionKind.referral_gift))
    {
        PortfolioTransactions.Add(new PortfolioTransaction
        {
            TransactionType = TransactionType.REFERAL,
            BrokerType = BrokerType.CRYPTOCOM,
            CreatedOn = cryptoRecordReferal.Timestamp,
            SymbolTemp = cryptoRecordReferal.Currency,
            AmountOfShares = cryptoRecordReferal.Amount,
            PricePerShare = 0,
            TotalCosts = 0
        });
    }


   

    Console.WriteLine($"----- RECORDS PROCESSED : {TotalRecords} RECORDS TOTAL {cryptoRecords.Count} -----");
    Console.WriteLine();

    foreach (var portfolioRecord in PortfolioTransactions)
    {
        Console.WriteLine($"Op {portfolioRecord.CreatedOn} heb je een {portfolioRecord.TransactionType} gedaan van {portfolioRecord.AmountOfShares} {portfolioRecord.SymbolTemp} aan {portfolioRecord.PricePerShare} euro voor in totaal aan {portfolioRecord.TotalCosts}");
    }

    Console.WriteLine("crypto.com generated");
}


Console.ReadLine();