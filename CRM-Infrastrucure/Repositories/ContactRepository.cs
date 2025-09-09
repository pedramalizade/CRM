namespace CRM_Infrastrucure.Repositories
{
    public class ContactRepository : IContactRepository
    {
        private readonly CRMContext _context;

        public ContactRepository(CRMContext context)
        {
            _context = context;
        }

        public async Task<List<Contact>> GetAllAsync(string? filter = null)
        {
            var query = _context.Contact
                .AsNoTracking()
                .Where(c => c.IsDeleted == 0)
                .OrderBy(c => c.Id)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(c => c.Surname.Contains(filter));
            }

            return await query.ToListAsync();
        }

        public async Task<Contact?> GetByIdAsync(int id)
        {
            return await _context.Contact
                .FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == 0);
        }

        public async Task AddAsync(Contact contact)
        {
            contact.IsDeleted = 0;
            _context.Contact.Add(contact);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Contact contact)
        {
            _context.Contact.Update(contact);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(Contact contact)
        {
            contact.IsDeleted = 1;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Contact.AnyAsync(c => c.Id == id && c.IsDeleted == 0);
        }
    }
}
