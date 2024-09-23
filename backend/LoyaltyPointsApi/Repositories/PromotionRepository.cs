using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyPointsApi.Data;
using LoyaltyPointsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LoyaltyPointsApi.Repositories
{
    public class PromotionRepository(LoyaltyDbContext dbContext) : IPromotionRepository

    {
        public async Task<Promotion?> AddPromotion(Promotion promotion)
        {
            await dbContext.Promotions.AddAsync(promotion);
            await dbContext.SaveChangesAsync();
            return promotion;
        }

        public async Task<Promotion?> GetPromotion(Promotion promotion)
        {
              return await dbContext.Promotions.FirstOrDefaultAsync(r => r.Id == promotion.Id); ;
        }

        public async Task<List<Promotion>> GetRestaurantPromotions(Promotion promotion)
        {
            var result = await dbContext.Promotions.Where(p => p.RestaurantId == promotion.RestaurantId).ToListAsync();
            return result;
        }


        public async  Task<Promotion?> UpdatePromotion(Promotion promotion)
        {
            dbContext.Update(promotion);
            await dbContext.SaveChangesAsync();
            return promotion;
        }

    }
}