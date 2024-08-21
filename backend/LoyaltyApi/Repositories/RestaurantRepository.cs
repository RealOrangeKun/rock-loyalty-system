using LoyaltyApi.Data;
using LoyaltyApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LoyaltyApi.Repositories
{
    public class RestaurantRepository(RockDbContext dbContext) :IRestaurantRepository
    {
       public async Task<object> GetCreditPointsInfo(int restaurantId){
        var restaurant = await dbContext.Restaurant.FirstOrDefaultAsync(r => r.RestaurantId == restaurantId);
        var creditPointInfo = new {
            restaurant.CreditPointsBuyingRate,
            restaurant.CreditPointsSellingRate,
            restaurant.CreditPointsLifeTime,
        };

        return creditPointInfo;
        }

        public async Task UpdateCreditPointsInfo(Restaurant restaurant){
            dbContext.Restaurant.Attach(restaurant);
            dbContext.Entry(restaurant).Property(r => r.CreditPointsBuyingRate).IsModified = true;
            dbContext.Entry(restaurant).Property(r => r.CreditPointsSellingRate).IsModified = true;
            dbContext.Entry(restaurant).Property(r => r.CreditPointsLifeTime).IsModified = true;
            dbContext.Entry(restaurant).Property(r => r.VoucherLifeTime).IsModified = true;

            await dbContext.SaveChangesAsync();
        }
       }

        
    }
