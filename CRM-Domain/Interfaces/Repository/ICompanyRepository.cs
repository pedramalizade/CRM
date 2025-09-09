using CRM.Domain.Entities;

namespace CRM_Domain.Interfaces.Repository
{
    public interface ICompanyRepository
    {
        Task<Company?> GetByIdAsync(int id);
        Task<List<Company>> GetAllAsync();
        Task AddAsync(Company company);
        Task UpdateAsync(Company company);
        Task DeleteAsync(Company company);
        Task<bool> ExistsAsync(int id);
        IQueryable<Company> Query();
    }
}
