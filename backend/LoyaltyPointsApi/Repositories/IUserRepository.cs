using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyPointsApi.Models;

namespace LoyaltyPointsApi.Repositories
{
    public interface IUserRepository
    {
        public Task<User?> GetUser(User user);

        
    }
}