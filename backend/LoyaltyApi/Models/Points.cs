namespace LoyaltyApi.Models
{
    public class Points
    {
        public required int CustomerId { get; set; }
        public required int RestaurantId { get; set; }
        public required int TransactionId { get; set; }

        public int LoyaltyPoints { get; set; }
        public DateTime DateOfCreation { get; set; }
        public int CreditPoints { get; set; }

    }
}