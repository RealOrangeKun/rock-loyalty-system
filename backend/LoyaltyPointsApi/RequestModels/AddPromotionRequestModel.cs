using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoyaltyPointsApi.RequestModels
{
    public class AddPromotionRequestModel
    {
        public int RestaurantId { get; set; }
        public string PromoCode { get; set; }
        public int ThresholdId { get; set; }
    }
}