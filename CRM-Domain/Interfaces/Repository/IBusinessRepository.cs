namespace CRM_Domain.Interfaces.Repository
{
    public interface IBusinessRepository
    {
        Task<List<Business>> GetAllAsync();
        Task<Business?> GetByIdAsync(int id);
        Task<Business> AddAsync(Business business);
        Task<Business> UpdateAsync(Business business);
        Task<bool> ExistsAsync(int id);
    }
}
