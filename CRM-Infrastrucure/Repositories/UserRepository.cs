namespace CRM.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly CRMContext _context;

        public UserRepository(CRMContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByLoginAsync(string login)
        {
            return await _context.User.FirstOrDefaultAsync(u => u.Login == login && u.IsDeleted == 0);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.User.FirstOrDefaultAsync(u => u.Id == id && u.IsDeleted == 0);
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _context.User.Where(u => u.IsDeleted == 0).ToListAsync();
        }

        public async Task AddAsync(User user)
        {
            await _context.User.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.User.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user != null)
            {
                user.IsDeleted = 1; // حذف منطقی
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsByLoginAsync(string login)
        {
            return await _context.User.AnyAsync(u => u.Login == login && u.IsDeleted == 0);
        }

        public async Task<int> CountAdminsAsync()
        {
            return await _context.User.CountAsync(u => u.RoleId == 1 && u.IsDeleted == 0);
        }
    }
}
