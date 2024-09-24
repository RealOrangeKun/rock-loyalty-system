using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyPointsApi.Data;
using LoyaltyPointsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LoyaltyPointsApi.Repositories
{
    public class RestaurantRepository(LoyaltyDbContext dbContext) : IRestaurantRepository
    {
        public async Task  <RestaurantSettings?> GetRestaurant(RestaurantSettings restaurant)
        {
            return await dbContext.RestaurantSettings.FirstOrDefaultAsync(id => id.RestaurantId == restaurant.RestaurantId); 

        }


        public async Task<RestaurantSettings> AddRestaurantSettings(RestaurantSettings restaurant)
        {
            await dbContext.RestaurantSettings.AddAsync(restaurant);
            await dbContext.SaveChangesAsync();
            return restaurant;
        }

        public async Task<RestaurantSettings> UpdateRestaurant(RestaurantSettings restaurant)
        {
            dbContext.RestaurantSettings.Update(restaurant);
            await dbContext.SaveChangesAsync();
            return restaurant;
        }
    }
}