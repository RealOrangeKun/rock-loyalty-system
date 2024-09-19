using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyPointsApi.Data;
using LoyaltyPointsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LoyaltyPointsApi.Repositories
{
    public class UserRepository(LoyaltyDbContext dbContext) : IUserRepository
    {
        public async Task AddUser(User user)
        {   
            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();
        }

        public async Task<User?> GetUser(User user)
        {
            var result =await dbContext.Users.FirstOrDefaultAsync(u => u.CustomerId == user.CustomerId && u.RestaurantId == user.RestaurantId);
            return result;
        }

        public async Task<User?> UpdateUser(User user)
        {
            dbContext.Users.Update(user);
            await dbContext.SaveChangesAsync();
            return user;
        }
    }
}