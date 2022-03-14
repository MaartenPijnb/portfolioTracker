using Microsoft.EntityFrameworkCore;

namespace PortfolioTracker.Model
{
    public class MPortfolioDBContext : DbContext
    {

        public MPortfolioDBContext(DbContextOptions<MPortfolioDBContext> options):base(options)
        {

        }
        public DbSet<PortfolioTransaction> Transactions { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<API> APIs { get; set; }
        public DbSet<PortfolioHistory> PortfolioHistory{ get; set; }
        public DbSet<Portfolio> Portfolio { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PortfolioTransaction>().ToTable("Transactions");
            modelBuilder.Entity<API>().ToTable("APIs").HasData(
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
                new Asset { 
                     ISN = "IE00B4L5Y983",
                     SymbolForApi= "IWDA.AS",
                     Name = "iShares Core MSCI World UCITS ETF USD (Acc)",
                     UpdatedOn = DateTime.Now,
                     AssetId = 1,
                     APIId = 1
                },
                new Asset
                {
                    ISN = "IE00B4L5YC18",
                    SymbolForApi = "IEMA.AS",
                    Name = "iShares MSCI EM UCITS ETF USD (Acc)",
                    UpdatedOn = DateTime.Now,
                    AssetId = 2,
                    APIId = 1
                }
                );

          
        }
    }
}