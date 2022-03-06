// See https://aka.ms/new-console-template for more information
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using PortfolioTracker.Degiro.Runner.Model;
using System.Globalization;
using static PortfolioTracker.Degiro.Runner.Model.DegiroRecord;

Console.WriteLine("Hello, World!");

var csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture) { 
    Delimiter = ",",
    HasHeaderRecord = true
};

using (var reader = new StreamReader("C:\\PortfolioFiles\\Account.csv"))
using (var csv = new CsvReader(reader, csvConfiguration))
{
    csv.Context.RegisterClassMap<DegiroRecordMap>();
      
    var records = csv.GetRecords<DegiroRecord>().ToList();
    records.ForEach(record => global::System.Console.WriteLine($"GUID: {(record.OrderID != null ? record.OrderID : "derp") } Omschrijving: {record.Description}"));
}


Console.ReadLine();