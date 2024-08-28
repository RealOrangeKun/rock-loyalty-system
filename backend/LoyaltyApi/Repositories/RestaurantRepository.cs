using System.Data;
using LoyaltyApi.Data;
using LoyaltyApi.Models;
using LoyaltyApi.RequestModels;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;

namespace LoyaltyApi.Repositories
{
    public class RestaurantRepository(RockDbContext dbContext) : IRestaurantRepository
    {
        //Create Method
        public async Task CreateRestaurant(Restaurant restaurant)
        {
            await dbContext.Restaurants.AddAsync(restaurant);

            await dbContext.SaveChangesAsync();
        }
        //Get Method
        public async Task<Restaurant?> GetRestaurantInfo(int restaurantId)
        {
            var restaurant = await dbContext.Restaurants.FirstOrDefaultAsync(r => r.RestaurantId == restaurantId);
            return restaurant;
        }
        //Update Method
        public async Task UpdateRestaurant(int RestaurantId ,Restaurant requestRestaurant)
        {
            //If this doesn't work
            var existingRestaurant = await dbContext.Restaurants.FirstOrDefaultAsync(r => r.RestaurantId == RestaurantId);
            if (existingRestaurant != null)
            {
                existingRestaurant.CreditPointsLifeTime = requestRestaurant.CreditPointsLifeTime;
                existingRestaurant.CreditPointsSellingRate = requestRestaurant.CreditPointsSellingRate;   
                existingRestaurant.CreditPointsBuyingRate = requestRestaurant.CreditPointsBuyingRate;
                existingRestaurant.VoucherLifeTime = requestRestaurant.VoucherLifeTime;
                await dbContext.SaveChangesAsync();
            }

            //Try This if the above doesn't work bas deh ht3ml update lel restaurant kolo i can handle it lw the above doesn't work

            // dbContext.Restaurants.Update(restaurant);
            // await dbContext.SaveChangesAsync();
        }
      
    }

}
