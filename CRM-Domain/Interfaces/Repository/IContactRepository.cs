namespace CRM_Domain.Interfaces.Repository
{
    public interface IContactRepository
    {
        Task<List<Contact>> GetAllAsync(string? filter = null);
        Task<Contact?> GetByIdAsync(int id);
        Task AddAsync(Contact contact);
        Task UpdateAsync(Contact contact);
        Task SoftDeleteAsync(Contact contact);
        Task<bool> ExistsAsync(int id);
    }
}
