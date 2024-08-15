namespace LoyaltyApi.Config
{
    public class JwtOptions
    {
        public required string SigningKey { get; set; }
        public required int ExpirationInMinutes { get; set; }
    }
}