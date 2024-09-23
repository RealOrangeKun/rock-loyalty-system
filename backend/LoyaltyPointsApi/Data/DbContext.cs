using LoyaltyPointsApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LoyaltyPointsApi.Data
{
    public class LoyaltyDbContext : DbContext
    {
        public LoyaltyDbContext(DbContextOptions<LoyaltyDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<RestaurantSettings> ResturantSettings { get; set; }
        public DbSet<LoyaltyPoints> LoyaltyPoints { get; set; }
        public DbSet<Threshold> Thresholds { get; set; }
        public DbSet<ApiKey> ApiKeys { get; set; }
        public DbSet<Promotion> Promotions { get; set; }

        // Fluent API configurations in OnModelCreating
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RestaurantSettings>()
                .HasKey(r => r.RestaurantId);

            modelBuilder.Entity<Threshold>()
                .HasKey(r =>  r.ThresholdId );
            modelBuilder.Entity<Threshold>()
                .Property(r => r.ThresholdId).ValueGeneratedOnAdd();

           
            modelBuilder.Entity<LoyaltyPoints>()
                .HasKey(r =>  r.TransactionId );
            modelBuilder.Entity<LoyaltyPoints>()
                .Property(r => r.TransactionId).ValueGeneratedOnAdd();
            modelBuilder.Entity<LoyaltyPoints>()
                .HasIndex(r => new { r.RestaurantId, r.CustomerId });

        
            modelBuilder.Entity<Promotion>()
                .HasKey(r =>  r.Id );
            modelBuilder.Entity<Promotion>()
            .Property(r => r.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Promotion>()
                .HasOne<Threshold>()
                .WithMany()
                .HasForeignKey(r => r.ThresholdId);



            modelBuilder.Entity<ApiKey>()
            .HasKey(r => r.Key);
            modelBuilder.Entity<ApiKey>()
                .HasIndex(r => r.RestaurantId)
                .IsUnique();
            
            base.OnModelCreating(modelBuilder);
        }
    }
}