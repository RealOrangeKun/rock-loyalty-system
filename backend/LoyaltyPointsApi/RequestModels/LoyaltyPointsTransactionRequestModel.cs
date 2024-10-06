namespace LoyaltyPointsApi.RequestModels
{
    public class LoyaltyPointsTransactionRequestModel
    {

        public int ReceiptId { get; set; }

        public required int CustomerId { get; set; }

        public required int RestaurantId { get; set; }


        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
        
    }
}