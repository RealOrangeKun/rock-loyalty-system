


using LoyaltyPointsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LoyaltyPointsApi.Data
{
    public class LoyaltyDbContext(DbContextOptions<LoyaltyDbContext> options) : DbContext(options)
    {
        DbSet<ResturantSettings> ResturantSettings { get; set; }

        DbSet<Threshold> Thresholds { get; set; }

        DbSet<Transaction> Transactions { get; set; }

        DbSet<ApiKey> ApiKeys { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<ResturantSettings>()
                .HasKey(r => r.ResturantId);

            modelBuilder.Entity<Threshold>()
                .HasKey(r => new { r.ResturantId, r.ThresholdName });

            modelBuilder.Entity<Transaction>()
                .HasKey(r => r.TransactionId);
            modelBuilder.Entity<Transaction>()
                .Property(r => r.TransactionId).ValueGeneratedOnAdd();

            modelBuilder.Entity<Transaction>()
                .HasIndex(r => new { r.ResturantId, r.CustomerId });

            modelBuilder.Entity<ApiKey>()
                .HasKey(r => r.Key);

            base.OnModelCreating(modelBuilder);
        }
    }
}