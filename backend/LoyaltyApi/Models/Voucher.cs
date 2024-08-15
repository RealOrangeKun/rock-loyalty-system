using System.ComponentModel.DataAnnotations;


namespace LoyaltyApi.Models
{
    public class Voucher
    {
        [Key]
        public required int Id { get; set; }

        public required int RestaurantId { get; set; }

        public required int CustomerId { get; set; }

        public required string Code { get; set; }
        public required int Value { get; set; }
        public required DateTime ExpirationDate { get; set; }
        public required bool IsUsed { get; set; }
    }
}