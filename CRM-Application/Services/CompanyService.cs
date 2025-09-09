using CRM.Application.Helpers;
using CRM.Domain.Entities;
using CRM_Domain.Interfaces.Repository;
using CRM_Domain.Interfaces.Service;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;             

namespace CRM_Application.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;

        public CompanyService(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        public async Task<Company?> GetCompanyAsync(int id, int userId)
        {
            var company = await _companyRepository.GetByIdAsync(id);
            if (company == null || company.UserId != userId)
                return null;
            return company;
        }

        public async Task<CRM_Domain.DTOs.PagedResult<Company>> GetCompaniesAsync(string start, string end, string category, int[] selected, int page, int pageSize, string sortExpression)
        {
            var qry = _companyRepository.Query();

            DateTime? startDate = !string.IsNullOrEmpty(start) && PersianDateHelper.IsValidPersianDate(start)
                ? PersianDateHelper.ToGregorianDate(start)
                : null;

            DateTime? endDate = !string.IsNullOrEmpty(end) && PersianDateHelper.IsValidPersianDate(end)
                ? PersianDateHelper.ToGregorianDate(end)
                : null;

            if (startDate != null)
                qry = qry.Where(c => c.CreationDate >= startDate);
            if (endDate != null)
                qry = qry.Where(c => c.CreationDate <= endDate);

            if (selected != null && selected.Length > 0)
                qry = qry.Where(c => selected.Contains(c.BusinessId));

            if (!string.IsNullOrEmpty(sortExpression))
                qry = qry.OrderBy(sortExpression);
            else
                qry = qry.OrderBy(c => c.Id);

            var totalCount = await qry.CountAsync();
            var items = await qry.Skip((page - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToListAsync();

            return new CRM_Domain.DTOs.PagedResult<Company>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<Company> CreateCompanyAsync(Company company, int userId)
        {
            company.UserId = userId;
            company.CreationDate = DateTime.Now;
            company.IsDeleted = 0;

            await _companyRepository.AddAsync(company);
            return company;
        }

        public async Task<bool> UpdateCompanyAsync(int id, Company company, int userId)
        {
            var existing = await _companyRepository.GetByIdAsync(id);
            if (existing == null || existing.UserId != userId)
                return false;

            existing.Name = company.Name;
            existing.NIP = company.NIP;
            existing.Address = company.Address;
            existing.City = company.City;

            await _companyRepository.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> DeleteCompanyAsync(int id, int userId)
        {
            var company = await _companyRepository.GetByIdAsync(id);
            if (company == null || company.UserId != userId)
                return false;

            await _companyRepository.DeleteAsync(company);
            return true;
        }
    }
}
