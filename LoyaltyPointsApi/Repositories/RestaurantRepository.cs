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
        public async Task  <RestaurantSettings?> GetRestaurant(int RestaurantId)
        {
            RestaurantSettings restaurant = await dbContext.ResturantSettings.FirstOrDefaultAsync(id => id.RestaurantId == RestaurantId); 

            return restaurant;
        }


        public async Task AddRestaurantSettings(RestaurantSettings restaurant)
        {
            await dbContext.ResturantSettings.AddAsync(restaurant);
            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateRestaurant(RestaurantSettings restaurant)
        {
            dbContext.ResturantSettings.Update(restaurant);
            await dbContext.SaveChangesAsync();
        }
    }
}