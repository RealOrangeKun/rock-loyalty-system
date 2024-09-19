using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyPointsApi.Models;
using LoyaltyPointsApi.RequestModels;

namespace LoyaltyPointsApi.Services
{
    public interface IUserService
    {
        public Task<User?> GetUser(UserRequestModel userRequestModel);
        public Task<User?> UpdateUser(UserRequestModel userRequestModel);
        public Task AddUser(AddUserRequestModel userRequestModel);
    }
}