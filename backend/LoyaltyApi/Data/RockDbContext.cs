﻿using Microsoft.EntityFrameworkCore;
using LoyaltyApi.Models;

namespace LoyaltyApi.Data
{
    public class RockDbContext(DbContextOptions<RockDbContext> dbContextOptions) : DbContext(dbContextOptions)
    {
        public DbSet<Token> Tokens { get; set; }

        public DbSet<CreditPointsTransaction> CreditPointsTransactions { get; set; }

        public DbSet<CreditPointsTransactionDetail> CreditPointsTransactionsDetails { get; set; }

        public DbSet<Restaurant> Restaurants { get; set; }

        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<Password> Passwords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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

            modelBuilder.Entity<Voucher>()
                .HasIndex(v => new {v.CustomerId, v.RestaurantId});
            
            modelBuilder.Entity<Voucher>()
                .HasIndex(v => v.DateOfCreation);
            
            modelBuilder.Entity<CreditPointsTransaction>()
                .HasKey(p => p.TransactionId);
            
            modelBuilder.Entity<CreditPointsTransaction>()
                .HasIndex(p => p.ReceiptId);
            
            modelBuilder.Entity<CreditPointsTransaction>()
                .HasIndex(p => p.TransactionDate);
            
            modelBuilder.Entity<CreditPointsTransaction>()
                .HasIndex(p => new {p.RestaurantId, p.CustomerId});
            
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
                .HasIndex(d => d.TransactionId);
            
            modelBuilder.Entity<CreditPointsTransactionDetail>()
                .HasIndex(d => d.EarnTransactionId);

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