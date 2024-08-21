using System.Data;
using LoyaltyApi.Data;
using LoyaltyApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LoyaltyApi.Repositories
{
    public class RestaurantRepository(RockDbContext dbContext) :IRestaurantRepository
    {
       public async Task<Restaurant> GetRestaurantInfo(int restaurantId){
        var restaurant = await dbContext.Restaurant.FirstOrDefaultAsync(r => r.RestaurantId == restaurantId);
        return restaurant ?? throw new DataException("Restaurant not found");
        }

        public async Task UpdateCreditBuyingRate(Restaurant restaurant)
        {
            dbContext.Restaurant.Attach(restaurant);
            dbContext.Entry(restaurant).Property(r => r.CreditPointsBuyingRate).IsModified = true;

            await dbContext.SaveChangesAsync();
        }


        public async Task CreditPointsLifeTime(Restaurant restaurant){
            dbContext.Restaurant.Attach(restaurant);
            dbContext.Entry(restaurant).Property(r => r.CreditPointsLifeTime).IsModified = true;
            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateCreditSellingRate(Restaurant restaurant)
        {
            dbContext.Restaurant.Attach(restaurant);
            dbContext.Entry(restaurant).Property(r => r.CreditPointsSellingRate).IsModified = true;
            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateVoucherLifeTime(Restaurant restaurant)
        {
        dbContext.Restaurant.Attach(restaurant);
         
        dbContext.Entry(restaurant).Property(r => r.VoucherLifeTime).IsModified = true;

            await dbContext.SaveChangesAsync();
        }
    }

    }
