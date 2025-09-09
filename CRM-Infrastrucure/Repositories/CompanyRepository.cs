namespace CRM_Infrastrucure.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly CRMContext _context;

        public CompanyRepository(CRMContext context)
        {
            _context = context;
        }

        public async Task<Company?> GetByIdAsync(int id)
        {
            return await _context.Company.FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == 0);
        }

        public async Task<List<Company>> GetAllAsync()
        {
            return await _context.Company.Where(c => c.IsDeleted == 0).ToListAsync();
        }

        public async Task AddAsync(Company company)
        {
            await _context.Company.AddAsync(company);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Company company)
        {
            _context.Company.Update(company);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Company company)
        {
            company.IsDeleted = 1;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Company.AnyAsync(c => c.Id == id && c.IsDeleted == 0);
        }

        public IQueryable<Company> Query()
        {
            return _context.Company.AsQueryable().Where(c => c.IsDeleted == 0);
        }
    }
}
