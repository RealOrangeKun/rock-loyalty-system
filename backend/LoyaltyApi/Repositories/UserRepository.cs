using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoyaltyApi.Repositories
{
    public class UserRepository : IUserRepository
    {
        public Task CreateUserAsync(int user)
        {
            throw new NotImplementedException();
        }

        public Task GetUserAsync(int userId, int restaurantId)
        {
            throw new NotImplementedException();
        }
    }
}