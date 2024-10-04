using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyPointsApi.Data;
using LoyaltyPointsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LoyaltyPointsApi.Repositories
{
    public class RestaurantRepository(LoyaltyDbContext dbContext,
        ILogger<RestaurantRepository> logger) : IRestaurantRepository
    {
        public async Task<RestaurantSettings?> GetRestaurant(RestaurantSettings restaurant)
        {
            logger.LogInformation("Getting Restaurant: {restaurantId}", restaurant.RestaurantId);
            return await dbContext.RestaurantSettings.FirstOrDefaultAsync(id => id.RestaurantId == restaurant.RestaurantId);

        }


        public async Task<RestaurantSettings> AddRestaurantSettings(RestaurantSettings restaurant)
        {
            logger.LogInformation("Adding Restaurant: {restaurantId}", restaurant.RestaurantId);
            await dbContext.RestaurantSettings.AddAsync(restaurant);
            await dbContext.SaveChangesAsync();
            return restaurant;
        }

        public async Task<RestaurantSettings> UpdateRestaurant(RestaurantSettings restaurant)
        {
            logger.LogInformation("Updating Restaurant: {restaurantId}", restaurant.RestaurantId);
            dbContext.RestaurantSettings.Update(restaurant);
            await dbContext.SaveChangesAsync();
            return restaurant;
        }

        public async Task<List<RestaurantSettings>> GetAllRestaurants()
        {
            logger.LogInformation("Getting all Restaurants");
            return await dbContext.RestaurantSettings.ToListAsync();
        }
    }
}