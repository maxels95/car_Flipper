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

            // Exempel: Förhindra att samma annons-URL läggs till flera gånger
            modelBuilder.Entity<Ad>()
                .HasIndex(a => a.Url)
                .IsUnique();
        }
    }
}
