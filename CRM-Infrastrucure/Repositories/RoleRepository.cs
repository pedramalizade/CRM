namespace CRM.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly CRMContext _context;

        public RoleRepository(CRMContext context)
        {
            _context = context;
        }

        public async Task<Role?> GetByIdAsync(int id)
        {
            return await _context.Role.FirstOrDefaultAsync(r => r.Id == id);
        }
    }
}
