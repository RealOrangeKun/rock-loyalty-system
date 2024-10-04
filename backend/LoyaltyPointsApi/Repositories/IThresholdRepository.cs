using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyPointsApi.Models;
using Microsoft.AspNetCore.SignalR;

namespace LoyaltyPointsApi.Repositories
{
    public interface IThresholdRepository
    {
        public Task<List<Threshold>> GetRestaurantThresholds(Threshold threshold);

        public Task<Threshold?> GetRestaurantThreshold(Threshold threshold);

        public Task<Threshold> AddThreshold(Threshold threshold);

        public Task<Threshold> UpdateThreshold(Threshold threshold);

        public Task DeleteThreshold(Threshold threshold);
    }
}