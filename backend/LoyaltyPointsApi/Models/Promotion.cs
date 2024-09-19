using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoyaltyPointsApi.Models
{
    public class Promotion
    {
        public int Id { get; set; }
        public int RestaurantId { get; set; }
        public string PromoCode { get; set; }
        public int ThresholdId { get; set; }
    }
}