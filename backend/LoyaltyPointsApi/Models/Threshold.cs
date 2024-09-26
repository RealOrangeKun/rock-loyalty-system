using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoyaltyPointsApi.Models
{
    public class Threshold
    {
        public int ThresholdId { get; set; }
        public required int RestaurantId { get; set; }

        public string ThresholdName { get; set; }

        public int MinimumPoints { get; set; }
        
        public List<Promotion> Promotions { get; set; } }
}