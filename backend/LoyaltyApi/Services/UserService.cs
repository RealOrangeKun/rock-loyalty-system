using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoyaltyApi.Services
{
    public class UserService : IUserService
    {
        public Task CreateUserAsync(int user)
        {
            throw new NotImplementedException();
        }

        public Task GetAndValidateUserAsync(string username, string password)
        {
            throw new NotImplementedException();
        }
    }
}