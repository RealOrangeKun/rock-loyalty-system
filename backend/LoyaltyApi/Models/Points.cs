using System.ComponentModel.DataAnnotations.Schema;

namespace LoyaltyApi.Models
{
    public class Points
    {
        public required int CustomerId { get; set; }
        [ForeignKey("Restaurant")]
        public required int RestaurantId { get; set; }
        public required int TransactionId { get; set; }

        public int LoyaltyPoints { get; set; }
        public DateTime DateOfCreation { get; set; }
        public int CreditPoints { get; set; }
        public virtual Restaurant Restaurant { get; set; }

    }
}