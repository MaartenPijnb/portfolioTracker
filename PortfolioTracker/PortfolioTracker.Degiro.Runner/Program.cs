// See https://aka.ms/new-console-template for more information
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using PortfolioTracker.Degiro.Runner.Model;
using PortfolioTracker.Model;
using PortfolioTracker.Model.Common;
using System.Globalization;
using static PortfolioTracker.Degiro.Runner.Model.DegiroRecord;

Console.WriteLine("Hello, World!");

var csvConfiguration = new CsvConfiguration(CultureInfo.GetCultureInfo("nl-NL"))
{
    Delimiter = ",",
    HasHeaderRecord = true
};

using (var reader = new StreamReader("C:\\PortfolioFiles\\Account.csv"))
using (var csv = new CsvReader(reader, csvConfiguration))
{
    csv.Context.RegisterClassMap<DegiroRecordMap>();

    var records = csv.GetRecords<DegiroRecord>().ToList();
    records.ForEach(record => global::System.Console.WriteLine($"GUID: {(record.OrderID != null ? record.OrderID : "derp") } Omschrijving: {record.Description}"));

    using (var dbConext = new MPortfolioDBContext(new Microsoft.EntityFrameworkCore.DbContextOptions<MPortfolioDBContext>()
    {
        
    }))
    {
        foreach (var record in records.Where(r => r.OrderID != null).GroupBy(x => x.OrderID))
        {
            var transaction = new PortfolioTransaction();
            var isBasicInfoFilled = false;
            foreach (var degiroOrderRecord in record)
            {
                if (!isBasicInfoFilled)
                {
                    transaction.CreatedOn = degiroOrderRecord.CreatedOnDate;
                    transaction.OrderId = degiroOrderRecord.OrderID;
                    transaction.AssetId = dbConext.Assets.Single(x => x.ISN == degiroOrderRecord.ISIN).AssetId;
                    transaction.CurencyType = degiroOrderRecord.Currency.Value;
                    
                    isBasicInfoFilled = true;
                }
                if (degiroOrderRecord.Description.Contains("Transactiebelasting"))
                {
                    transaction.TaxesCosts = degiroOrderRecord.TotalPrice.HasValue ? degiroOrderRecord.TotalPrice.Value * -1: 0;
                }
                
                if (degiroOrderRecord.Description.Contains("Transactiekosten"))
                {
                    transaction.TransactionCosts = degiroOrderRecord.TotalPrice.HasValue ? degiroOrderRecord.TotalPrice.Value * -1 : 0;
                }
                
                if (degiroOrderRecord.Description.Contains("Koop"))
                {
                    transaction.TransactionType = TransactionType.BUY;
                    //Koop 20 @ 35,55 EUR
                    var descriptionContainingAssetInformation = degiroOrderRecord.Description.Substring(4, degiroOrderRecord.Description.Length-4);
                    //20 @ 35,55 EUR
                    descriptionContainingAssetInformation = descriptionContainingAssetInformation.Replace(" ", "");
                    //20@35,55EUR
                    descriptionContainingAssetInformation = descriptionContainingAssetInformation.Replace("EUR", "");
                    //20@35,55
                    var assetInformation = descriptionContainingAssetInformation.Split("@");
                    transaction.AmountOfShares =  decimal.Parse(assetInformation[0]);
                    transaction.PricePerShare = decimal.Parse(assetInformation[1]);
                }
            }
            transaction.TotalCosts = transaction.AmountOfShares * transaction.PricePerShare + transaction.TaxesCosts + transaction.TransactionCosts;

            dbConext.Transactions.Add(transaction);
        }
        dbConext.SaveChanges();
    };

}


Console.ReadLine();
