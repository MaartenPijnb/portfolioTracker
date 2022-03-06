using Microsoft.EntityFrameworkCore;

namespace PortfolioTracker.Model
{
    public class MPortfolioDBContext : DbContext
    {

        public MPortfolioDBContext(DbContextOptions<MPortfolioDBContext> options):base(options)
        {

        }
        public DbSet<PortfolioTransaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PortfolioTransaction>().ToTable("Transactions");
        }
    }
}