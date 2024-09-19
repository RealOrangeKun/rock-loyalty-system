using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LoyaltyApi.Data
{
    public class FrontendDbContext(DbContextOptions<FrontendDbContext> options) : DbContext(options)
    {
        public DbSet<Token> Tokens { get; set; }

        public DbSet<CreditPointsTransaction> CreditPointsTransactions { get; set; }

        public DbSet<CreditPointsTransactionDetail> CreditPointsTransactionsDetails { get; set; }

        public DbSet<Restaurant> Restaurants { get; set; }

        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<Password> Passwords { get; set; }
        public DbSet<User> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.RestaurantId);

            modelBuilder.Entity<Restaurant>()
                .HasKey(r => r.RestaurantId);

            modelBuilder.Entity<Token>()
                .HasKey(x => new { x.CustomerId, x.RestaurantId, x.TokenValue });

            modelBuilder.Entity<Token>()
                .Property(x => x.TokenType)
                .HasConversion<string>();

            modelBuilder.Entity<Voucher>()
                .HasKey(x => x.ShortCode);

            modelBuilder.Entity<Voucher>()
                .Property(v => v.IsUsed)
                .HasDefaultValue(false);

            modelBuilder.Entity<CreditPointsTransaction>()
                .HasKey(p => p.TransactionId);

            modelBuilder.Entity<CreditPointsTransaction>()
                .Property(p => p.TransactionId)
                .ValueGeneratedOnAdd();

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

            modelBuilder.Entity<Password>()
            .HasKey(p => new { p.CustomerId, p.RestaurantId });

            modelBuilder.Entity<Password>()
            .Property(p => p.Confirmed)
            .HasDefaultValue(false);

            base.OnModelCreating(modelBuilder);
        }
    }
}