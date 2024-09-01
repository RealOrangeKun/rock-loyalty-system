using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyApi.Data;
using LoyaltyApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LoyaltyApi.Repositories
{
    public class PasswordRepository(RockDbContext dbContext) : IPasswordRepository
    {
        public async Task<Password> CreatePasswordAsync(Password password)
        {
            await dbContext.Passwords.AddAsync(password);
            await dbContext.SaveChangesAsync();
            return password;
        }

        public async Task<Password?> GetPasswordAsync(Password password)
        {
            return await dbContext.Passwords.FirstOrDefaultAsync(p => p.CustomerId == password.CustomerId && p.RestaurantId == password.RestaurantId);
        }

        public async Task<Password> UpdatePasswordAsync(Password password)
        {
            dbContext.Passwords.Update(password);
            await dbContext.SaveChangesAsync();
            return password;
        }
    }
}