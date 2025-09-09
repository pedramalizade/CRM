namespace CRM_Infrastrucure.Repositories
{
    public class BusinessRepository : IBusinessRepository
    {
        private readonly CRMContext _context;

        public BusinessRepository(CRMContext context)
        {
            _context = context;
        }

        public async Task<List<Business>> GetAllAsync()
        {
            return await _context.Business.ToListAsync();
        }

        public async Task<Business?> GetByIdAsync(int id)
        {
            return await _context.Business.FindAsync(id);
        }

        public async Task<Business> AddAsync(Business business)
        {
            _context.Business.Add(business);
            await _context.SaveChangesAsync();
            return business;
        }

        public async Task<Business> UpdateAsync(Business business)
        {
            _context.Business.Update(business);
            await _context.SaveChangesAsync();
            return business;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Business.AnyAsync(b => b.Id == id);
        }
    }
}
