﻿using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PortfolioTracker.Degiro.Runner.Model;
using PortfolioTracker.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioTracker.Degiro.Runner.Controller
{
    public class DegiroController: IDegiroController
    {
        private readonly MPortfolioDBContext _dbContext;
        private readonly ILogger<DegiroController> _logger;

        public DegiroController(MPortfolioDBContext dbContext, ILogger<DegiroController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task ImportDegiro(StreamReader degiroCsvStream, long userId)
        {
            var csvConfiguration = new CsvConfiguration(CultureInfo.GetCultureInfo("nl-NL"))
            {
                Delimiter = ",",
                HasHeaderRecord = true
            };
            var allOrderIds = _dbContext.Transactions.Select(t => t.OrderId).ToList();
            using (var csv = new CsvReader(degiroCsvStream, csvConfiguration))
            {
                csv.Context.RegisterClassMap<DegiroRecordMap>();
                var records = csv.GetRecords<DegiroRecord>().ToList();


                    foreach (var record in records.Where(r => r.OrderID != null && !allOrderIds.Contains(r.OrderID)).GroupBy(x => x.OrderID))
                    {
                        var transaction = new PortfolioTransaction();
                        transaction.UserID = userId;
                        var isBasicInfoFilled = false;
                        foreach (var degiroOrderRecord in record)
                        {
                            if (!isBasicInfoFilled)
                            {
                                transaction.CreatedOn = degiroOrderRecord.CreatedOnDate;
                                transaction.OrderId = degiroOrderRecord.OrderID;
                                transaction.AssetId = _dbContext.Assets.Single(x => x.ISN == degiroOrderRecord.ISIN).AssetId;
                                transaction.CurencyType = degiroOrderRecord.Currency.Value;
                                transaction.BrokerType = BrokerType.DEGIRO;

                                isBasicInfoFilled = true;
                            }
                            if (degiroOrderRecord.Description.Contains("Transactiebelasting"))
                            {
                                transaction.TaxesCosts = degiroOrderRecord.TotalPrice.HasValue ? degiroOrderRecord.TotalPrice.Value * -1 : 0;
                            }

                            if (degiroOrderRecord.Description.Contains("Transactiekosten"))
                            {
                                transaction.TransactionCosts = degiroOrderRecord.TotalPrice.HasValue ? degiroOrderRecord.TotalPrice.Value * -1 : 0;
                            }

                            if (degiroOrderRecord.Description.Contains("Koop"))
                            {
                                transaction.TransactionType = TransactionType.BUY;
                                //Koop 20 @ 35,55 EUR
                                var descriptionContainingAssetInformation = degiroOrderRecord.Description.Substring(4, degiroOrderRecord.Description.Length - 4);
                                //20 @ 35,55 EUR
                                descriptionContainingAssetInformation = descriptionContainingAssetInformation.Replace(" ", "");
                                //20@35,55EUR
                                descriptionContainingAssetInformation = descriptionContainingAssetInformation.Replace("EUR", "");
                                descriptionContainingAssetInformation = descriptionContainingAssetInformation.Replace("USD", "");
                            //20@35,55
                            var assetInformation = descriptionContainingAssetInformation.Split("@");
                                transaction.AmountOfShares = decimal.Parse(assetInformation[0]);
                                transaction.PricePerShare = decimal.Parse(assetInformation[1]);
                            }
                        }
                        transaction.TotalCosts = transaction.AmountOfShares * transaction.PricePerShare + transaction.TaxesCosts + transaction.TransactionCosts;

                    _dbContext.Transactions.Add(transaction);
                    }

                foreach (var record in records.Where(r => r.Description.Contains("flatex Deposit") || r.Description.Contains("Sofort Deposit")))
                {
                    var accountBalance = new AccountBalance
                    {
                        BrokerType = BrokerType.DEGIRO,
                        CreatedOn = record.CreatedOnDate,
                        DepositType = DepositType.DEPOSIT,
                        Value = record.TotalPrice.Value,
                        UserID=userId
                    };
                    _dbContext.AccountBalance.Add(accountBalance);
                }
                _dbContext.SaveChanges();            
            }
        }
    }
}
