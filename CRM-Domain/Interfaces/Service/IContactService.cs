namespace CRM_Domain.Interfaces.Service
{
    public interface IContactService
    {
        Task<List<Contact>> GetAllAsync(string? filter = null);
        Task<Contact?> GetByIdAsync(int id);
        Task<Contact> CreateAsync(Contact contact, int userId);
        Task<Contact?> UpdateAsync(int id, Contact updatedContact, int userId);
        Task<bool> DeleteAsync(int id, int userId);
    }
}
