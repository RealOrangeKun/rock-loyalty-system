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
        public async Task<Restaurant?> GetRestaurantById(int restaurantId)
        {
            var restaurant = await dbContext.Restaurants.FirstOrDefaultAsync(r => r.RestaurantId == restaurantId);
            return restaurant;
        }

        public async Task<IEnumerable<Restaurant>> GetAllRestaurantsAsync()
        {
            return await dbContext.Restaurants.ToListAsync();
        }
        //Update Method
        public async Task UpdateRestaurant(Restaurant restaurant)
        {
            //If this doesn't work

            dbContext.Restaurants.Update(restaurant);
            await dbContext.SaveChangesAsync();

            //Try This if the above doesn't work bas deh ht3ml update lel restaurant kolo i can handle it lw the above doesn't work

            // dbContext.Restaurants.Update(restaurant);
            // await dbContext.SaveChangesAsync();
        }

    }

}
