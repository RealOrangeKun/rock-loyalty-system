
namespace LoyaltyApi.Repositories
{
    public interface IUserRepository
    {
        Task CreateUserAsync(int user);

        Task GetUserAsync(int userId, int restaurantId);

    }
}