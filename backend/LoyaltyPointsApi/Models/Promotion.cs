using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoyaltyPointsApi.Models
{
    public class Promotion
    {
        public int RestaurantId { get; set; }
        public string PromoCode { get; set; }
        public int ThresholdId { get; set; }
        public bool IsNotified { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Threshold Threshold { get; set; }
    }
}