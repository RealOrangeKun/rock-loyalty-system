namespace LoyaltyPointsApi.RequestModels
{
    public class PromotionRequestModel
    {        
        public int RestaurantId { get; set; }
        public string PromoCode { get; set; }
        public int ThresholdId { get; set; }
    }
}
