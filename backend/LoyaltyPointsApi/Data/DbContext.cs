


using LoyaltyPointsApi.Models;

using Microsoft.EntityFrameworkCore;

namespace LoyaltyPointsApi.Data
{
    public class LoyaltyDbContext(DbContextOptions<LoyaltyDbContext> options) : DbContext(options)
    {
        public DbSet<RestaurantSettings> ResturantSettings { get; set; }
        public DbSet<LoyaltyPoints> LoyaltyPoints { get; set; }

        public DbSet<Threshold> Thresholds { get; set; }

        public DbSet<ApiKey> ApiKeys { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Promotion> Promotions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<RestaurantSettings>()
                .HasKey(r => r.RestaurantId);

            modelBuilder.Entity<Threshold>()
                .HasKey(r => r.RestaurantId);
            modelBuilder.Entity<Threshold>()
            .HasKey(r => r.ThresholdId);

            modelBuilder.Entity<Threshold>()
            .Property(r => r.ThresholdId).ValueGeneratedOnAdd();

            modelBuilder.Entity<ApiKey>()
                .HasKey(r => r.Key);

            modelBuilder.Entity<ApiKey>()
                .HasIndex(r => r.RestaurantId)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasKey(r => r.CustomerId);
            modelBuilder.Entity<User>()
                .HasIndex().IsUnique();

            modelBuilder.Entity<LoyaltyPoints>()
                .HasKey(r => new { r.TransactionId });
            modelBuilder.Entity<LoyaltyPoints>()
                .Property(r => r.TransactionId).ValueGeneratedOnAdd();
            modelBuilder.Entity<LoyaltyPoints>()
                .HasIndex(r => new { r.RestaurantId, r.CustomerId });
            modelBuilder.Entity<Promotion>()
                .HasKey(r => r.Id);
            modelBuilder.Entity<Promotion>()
                .HasOne<Threshold>()
                .WithMany()
                .HasForeignKey(r => r.ThresholdId);
            base.OnModelCreating(modelBuilder);
        }
    }
}