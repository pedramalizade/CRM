using CRM.Domain.Entities;
using CRM_Domain.DTOs;

namespace CRM_Domain.Interfaces.Service
{
    public interface ICompanyService
    {
        Task<Company?> GetCompanyAsync(int id, int userId);
        Task<PagedResult<Company>> GetCompaniesAsync(string start, string end, string category, int[] selected, int page, int pageSize, string sortExpression);
        Task<Company> CreateCompanyAsync(Company company, int userId);
        Task<bool> UpdateCompanyAsync(int id, Company company, int userId);
        Task<bool> DeleteCompanyAsync(int id, int userId);
    }
}
