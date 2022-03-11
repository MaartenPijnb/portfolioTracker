﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PortfolioTracker.Model;

#nullable disable

namespace PortfolioTracker.Server.Migrations
{
    [DbContext(typeof(MPortfolioDBContext))]
    [Migration("20220311194236_Portfolio History tabel created")]
    partial class PortfolioHistorytabelcreated
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("PortfolioTracker.Model.API", b =>
                {
                    b.Property<int>("APIId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("APIId"), 1L, 1);

                    b.Property<string>("APIKey")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("APIName")
                        .HasColumnType("int");

                    b.Property<string>("BaseUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("APIId");

                    b.ToTable("APIs", (string)null);

                    b.HasData(
                        new
                        {
                            APIId = 1,
                            APIKey = "6PoQCJV9O17jeUPS81UDN1sHJ86gKB4RahYraKSS",
                            APIName = 0,
                            BaseUrl = "https://yfapi.net"
                        });
                });

            modelBuilder.Entity("PortfolioTracker.Model.Asset", b =>
                {
                    b.Property<int>("AssetId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AssetId"), 1L, 1);

                    b.Property<int>("APIId")
                        .HasColumnType("int");

                    b.Property<string>("ISN")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SymbolForApi")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("Value")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("AssetId");

                    b.HasIndex("APIId");

                    b.ToTable("Assets", (string)null);

                    b.HasData(
                        new
                        {
                            AssetId = 1,
                            APIId = 1,
                            ISN = "IE00B4L5Y983",
                            Name = "iShares Core MSCI World UCITS ETF USD (Acc)",
                            SymbolForApi = "IWDA.AS",
                            UpdatedOn = new DateTime(2022, 3, 11, 20, 42, 35, 88, DateTimeKind.Local).AddTicks(1953),
                            Value = 0m
                        },
                        new
                        {
                            AssetId = 2,
                            APIId = 1,
                            ISN = "IE00B4L5YC18",
                            Name = "iShares MSCI EM UCITS ETF USD (Acc)",
                            SymbolForApi = "IEMA.AS",
                            UpdatedOn = new DateTime(2022, 3, 11, 20, 42, 35, 88, DateTimeKind.Local).AddTicks(1982),
                            Value = 0m
                        });
                });

            modelBuilder.Entity("PortfolioTracker.Model.PortfolioHistory", b =>
                {
                    b.Property<int>("PortfolioHistoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PortfolioHistoryId"), 1L, 1);

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("Percentage")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("Profit")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("TotalInvestedPortfolioValue")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("TotalPortfolioValue")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("PortfolioHistoryId");

                    b.ToTable("PortfolioHistory");
                });

            modelBuilder.Entity("PortfolioTracker.Model.PortfolioTransaction", b =>
                {
                    b.Property<int>("PortfolioTransactionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PortfolioTransactionId"), 1L, 1);

                    b.Property<decimal>("AmountOfShares")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("AssetId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<int>("CurencyType")
                        .HasColumnType("int");

                    b.Property<Guid?>("OrderId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("PricePerShare")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("TaxesCosts")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("TotalCosts")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("TransactionCosts")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("TransactionType")
                        .HasColumnType("int");

                    b.HasKey("PortfolioTransactionId");

                    b.HasIndex("AssetId");

                    b.ToTable("Transactions", (string)null);
                });

            modelBuilder.Entity("PortfolioTracker.Model.Asset", b =>
                {
                    b.HasOne("PortfolioTracker.Model.API", "API")
                        .WithMany()
                        .HasForeignKey("APIId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("API");
                });

            modelBuilder.Entity("PortfolioTracker.Model.PortfolioTransaction", b =>
                {
                    b.HasOne("PortfolioTracker.Model.Asset", "Asset")
                        .WithMany()
                        .HasForeignKey("AssetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Asset");
                });
#pragma warning restore 612, 618
        }
    }
}