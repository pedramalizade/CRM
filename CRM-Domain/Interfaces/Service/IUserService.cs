namespace CRM_Domain.Interfaces.Service
{
    public interface IUserService
    {
        Task<bool> ValidateLoginAsync(string userName, string password);
        Task<string> GetRoleNameAsync(string userName);
        Task<List<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<bool> RegisterUserAsync(User user);
        Task<bool> UpdateUserAsync(User user);
        Task DeleteUserAsync(int id);
    }
}
