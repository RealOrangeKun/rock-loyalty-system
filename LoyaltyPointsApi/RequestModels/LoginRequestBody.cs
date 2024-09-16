namespace LoyaltyPointsApi.RequestModels
{
    public class LoginRequestBody
    {
        public required int RestaurantId { get; set; }
        public required string Password { get; set; }
        public required string Username { get; set; }
    }
}