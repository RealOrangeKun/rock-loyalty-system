using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoyaltyPointsApi.RequestModels
{
    public class ThresholdRequestModel
    {
        public int RestaurantId { get; set; }
        public required string ThresholdName { get; set; }

        public required int MinimumPoints { get; set; } 
    }
}