using System.Data;
using LoyaltyApi.Data;
using LoyaltyApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LoyaltyApi.Repositories
{
    public class RestaurantRepository(RockDbContext dbContext) :IRestaurantRepository
    {
        //Create Methods
        public async Task CreateRestaurant(Restaurant restaurant)
        {
            await dbContext.Restaurants.AddAsync(restaurant);            
        }
        //Get Methods
        public async Task<Restaurant> GetRestaurantInfo(int restaurantId){
        var restaurant = await dbContext.Restaurants.FirstOrDefaultAsync(r => r.RestaurantId == restaurantId);
        return restaurant ?? throw new DataException("Restaurant not found");
        }
        //Update Methods
        public async Task UpdateCreditBuyingRate(Restaurant restaurant)
        {
            //If this doesn't work
            var existingRestaurant = await dbContext.Restaurants.FirstOrDefaultAsync(r => r.RestaurantId == restaurant.RestaurantId);
             if (existingRestaurant != null)
             {
                existingRestaurant.CreditPointsBuyingRate = restaurant.CreditPointsBuyingRate;
                await dbContext.SaveChangesAsync();
            }

            //Try This if the above doesn't work bas deh ht3ml update lel restaurant kolo i can handle it lw the above doesn't work

            // dbContext.Restaurants.Update(restaurant);
            // await dbContext.SaveChangesAsync();
        }


        public async Task UpdateCreditPointsLifeTime(Restaurant restaurant){
            var existingRestaurant = await dbContext.Restaurants.FirstOrDefaultAsync(r => r.RestaurantId == restaurant.RestaurantId);
             if (existingRestaurant != null)
             {
                existingRestaurant.CreditPointsLifeTime = restaurant.CreditPointsLifeTime;
                await dbContext.SaveChangesAsync();
            }

            // dbContext.Restaurants.Update(restaurant);
            // await dbContext.SaveChangesAsync();
        }

        public async Task UpdateCreditSellingRate(Restaurant restaurant)
        {
            var existingRestaurant = await dbContext.Restaurants.FirstOrDefaultAsync(r => r.RestaurantId == restaurant.RestaurantId);
             if (existingRestaurant != null)
             {
                existingRestaurant.CreditPointsSellingRate = restaurant.CreditPointsSellingRate;
                await dbContext.SaveChangesAsync();
            }

            // dbContext.Restaurants.Update(restaurant);
            // await dbContext.SaveChangesAsync();
        }

        public async Task UpdateVoucherLifeTime(Restaurant restaurant)
        {
           var existingRestaurant = await dbContext.Restaurants.FirstOrDefaultAsync(r => r.RestaurantId == restaurant.RestaurantId);
             if (existingRestaurant != null)
             {
                existingRestaurant.VoucherLifeTime = restaurant.VoucherLifeTime;
                await dbContext.SaveChangesAsync();
            }



            // dbContext.Restaurants.Update(restaurant);
            // await dbContext.SaveChangesAsync();
        }
    }

    }
