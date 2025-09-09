namespace CRM_Domain.Interfaces.Service
{
    public interface IBusinessService
    {
        Task<List<Business>> GetAllAsync();
        Task<Business?> GetByIdAsync(int id);
        Task<Business> CreateAsync(Business business);
        Task<Business> UpdateAsync(Business business);
        Task<bool> ExistsAsync(int id);
    }
}
