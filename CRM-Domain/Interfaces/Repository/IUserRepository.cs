namespace CRM_Domain.Interfaces.Repository
{
    public interface IUserRepository
    {
        Task<User?> GetByLoginAsync(string login);
        Task<User?> GetByIdAsync(int id);
        Task<List<User>> GetAllAsync();
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(int id);
        Task<bool> ExistsByLoginAsync(string login);
        Task<int> CountAdminsAsync();
    }
}
