namespace LoyaltyApi.RequestModels
{
    public class LoginRequestBody
    {
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public required int RestaurantId { get; set; }

        
    }
}