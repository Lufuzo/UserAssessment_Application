using UserAssessment_Application.Entities;

namespace UserAssessment_Application.Repositories.IRepo
{
    public interface IUserRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByIdAsync(int id);
        Task<User> CreateUserAsync(User user);
        Task SaveChangesAsync();
    }
}
