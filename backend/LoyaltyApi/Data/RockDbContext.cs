using Microsoft.EntityFrameworkCore;
using LoyaltyApi.Models;

namespace LoyaltyApi.Data
{
    public class RockDbContext(DbContextOptions<RockDbContext> dbContextOptions) : DbContext(dbContextOptions)
    {
        public DbSet<Token> Tokens { get; set; }

        public DbSet<CreditPointsTransaction> CreditPointsTransactions { get; set; }

        public DbSet<Restaurant> Restaurants { get; set; }

        public DbSet<Voucher> Vouchers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Restaurant>()
                .HasKey(r => r.RestaurantId);

            modelBuilder.Entity<Token>()
                .HasKey(x => new { x.CustomerId, x.RestaurantId, x.TokenValue });

            modelBuilder.Entity<Voucher>()
                .HasKey(x => new { x.Id, x.RestaurantId, x.CustomerId, x.Code });

            modelBuilder.Entity<CreditPointsTransaction>()
                .HasKey(p => p.TransactionId);

            modelBuilder.Entity<CreditPointsTransaction>()
                .HasOne(p => p.Restaurant)
                .WithMany()
                .HasForeignKey(p => p.RestaurantId);

            modelBuilder.Entity<CreditPointsTransactionDetail>()
                .HasKey(d => d.DetailId);
            
            modelBuilder.Entity<CreditPointsTransactionDetail>()
                .Property(d => d.DetailId)
                .ValueGeneratedOnAdd();
            
            modelBuilder.Entity<CreditPointsTransactionDetail>()
                .HasOne(d => d.Transaction)
                .WithMany(p => p.CreditPointsTransactionDetails)
                .HasForeignKey(d => d.TransactionId);

            modelBuilder.Entity<CreditPointsTransaction>()
                .HasMany(p => p.CreditPointsTransactionDetails)
                .WithOne(d => d.Transaction)
                .HasForeignKey(d => d.TransactionId);

            modelBuilder.Entity<CreditPointsTransactionDetail>()
                .HasOne(d => d.EarnTransaction)
                .WithMany()
                .HasForeignKey(d => d.EarnTransactionId);

            modelBuilder.Entity<CreditPointsTransaction>()
                .Property(p => p.TransactionType)
                .HasConversion<string>(); // Store enum as string in the database

            base.OnModelCreating(modelBuilder);
        }
    }
}