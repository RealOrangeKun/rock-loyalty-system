using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyApi.Models;

namespace LoyaltyApi.Services
{
    public interface IUserService
    {
        Task GetAndValidateUserAsync(string phoneNumber, string password);
        Task<object> CreateUserAsync(User user);
    }
}