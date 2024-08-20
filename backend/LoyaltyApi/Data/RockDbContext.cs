using Microsoft.EntityFrameworkCore;
using LoyaltyApi.Models;

namespace LoyaltyApi.Data
{
    public class RockDbContext : DbContext
    {
        public DbSet<Token> Tokens { get; set; }
        public DbSet<Points> Points { get; set; }
        public DbSet<Restaurant> Restaurant { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Points>().HasKey(x => new { x.CustomerId, x.RestaurantId, x.TransactionId });
            modelBuilder.Entity<Token>().HasKey(x => new { x.CustomerId, x.RestaurantId, x.TokenValue });
            modelBuilder.Entity<Voucher>().HasKey(x=> new {x.Id,x.RestaurantId,x.CustomerId,x.Code});
            modelBuilder.Entity<Restaurant>().HasKey(x => x.RestaurantId);
            

        }
    }
}
