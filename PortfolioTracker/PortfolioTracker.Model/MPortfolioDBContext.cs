using Microsoft.EntityFrameworkCore;

namespace PortfolioTracker.Model
{
    public class MPortfolioDBContext : DbContext
    {

        public MPortfolioDBContext(DbContextOptions<MPortfolioDBContext> options) : base(options)
        {

        }
        public DbSet<PortfolioTransaction> Transactions { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<API> APIs { get; set; }
        public DbSet<PortfolioHistory> PortfolioHistory { get; set; }
        public DbSet<Portfolio> Portfolio { get; set; }
        public DbSet<AccountBalance> AccountBalance { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PortfolioTransaction>().ToTable("Transactions");
            modelBuilder.Entity<API>().ToTable("APIs").HasData(
              new API
              {
                  APIKey = "not applicable",
                  APIName = APIType.NOTAPPLICABLE,
                  BaseUrl = "not applicable",
                  APIId = 2
              },
               new API
               {
                   APIKey = "6PoQCJV9O17jeUPS81UDN1sHJ86gKB4RahYraKSS",
                   APIName = APIType.YAHOOFINANCE,
                   BaseUrl = "https://yfapi.net",
                   APIId = 1
               }
              );

            //Degiro import makes it impossible to create assets dynamically with yahoo finance api :(

            modelBuilder.Entity<Asset>().ToTable("Assets").HasData(
                new Asset
                {
                    ISN = "IE00B4L5Y983",
                    SymbolForApi = "IWDA.AS",
                    Name = "iShares Core MSCI World UCITS ETF USD (Acc)",
                    UpdatedOn = DateTime.Now,
                    AssetId = 1,
                    APIId = 1,
                    AssetType = AssetType.Etf
                },
                new Asset
                {
                    ISN = "IE00B4L5YC18",
                    SymbolForApi = "IEMA.AS",
                    Name = "iShares MSCI EM UCITS ETF USD (Acc)",
                    UpdatedOn = DateTime.Now,
                    AssetId = 2,
                    APIId = 1,
                    AssetType = AssetType.Etf
                }
                ,
                new Asset
                {
                    ISN = "BE0172903495",
                    SymbolForApi = "0P00000NFB.F",
                    Name = "Argenta Pensioenspaarfonds",
                    UpdatedOn = DateTime.Now,
                    AssetId = 3,
                    APIId = 1,
                    AssetType = AssetType.Pensioen
                },
                new Asset
                {
                    ISN = "Groepsverzekering IS",
                    SymbolForApi = "not applicable",
                    Name = "Groepsverzekering IS",
                    AssetId = 4,
                    APIId = 2,
                    UpdatedOn = DateTime.Now,
                    AssetType = AssetType.Groepsverzekering
                }
                );
        }

        protected override void ConfigureConventions(
            ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<decimal>()
                .HavePrecision(18, 8);
        }
    }
}