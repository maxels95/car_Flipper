using CarFlipper.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CarFlipper.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Ad> Ads { get; set; }
        public DbSet<PriceHistory> PriceHistories { get; set; }
        public DbSet<MarketPrice> MarketPrices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Ad>()
                .HasOne(ad => ad.MarketPrice)
                .WithMany(mp => mp.Ads)
                .HasForeignKey(ad => ad.MarketPriceId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
