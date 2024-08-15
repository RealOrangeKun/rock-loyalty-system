namespace LoyaltyApi.Models
{
    public class AuthBody
    {
        public string PhoneNumber { get; set; }

        public string Password { get; set; }

        public int RestaurantId { get; set; }
    }
}