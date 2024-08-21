
namespace LoyaltyApi.RequestModels
{
    public class CreateVoucherRequest
    {
        public int Points { get; set; }
        public int RestaurantId { get; set; }
        public int CustomerId { get; set; }
    }
}