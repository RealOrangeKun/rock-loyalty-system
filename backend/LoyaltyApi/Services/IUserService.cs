using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoyaltyApi.Services
{
    public interface IUserService
    {
        Task GetAndValidateUserAsync(string username, string password);
        Task CreateUserAsync(int user);
    }
}