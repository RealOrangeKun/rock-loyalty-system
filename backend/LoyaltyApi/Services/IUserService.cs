using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyApi.Models;

namespace LoyaltyApi.Services
{
    public interface IUserService
    {
        Task<User?> GetAndValidateUserAsync(string? phoneNumber, string? email, string? password, int restaurantId);
        Task<object> CreateUserAsync(User user);
    }
}