namespace CRM_Infrastrucure.Repositories
{
    public class NoteRepository : INoteRepository
    {
        private readonly CRMContext _context;

        public NoteRepository(CRMContext context)
        {
            _context = context;
        }

        public async Task<List<Note>> GetAllAsync()
        {
            return await _context.Note.Where(n => n.IsDeleted == 0).ToListAsync();
        }

        public async Task<Note?> GetByIdAsync(int id)
        {
            return await _context.Note.FirstOrDefaultAsync(n => n.Id == id && n.IsDeleted == 0);
        }

        public async Task AddAsync(Note note)
        {
            await _context.Note.AddAsync(note);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Note note)
        {
            _context.Note.Update(note);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var note = await _context.Note.FirstOrDefaultAsync(n => n.Id == id && n.IsDeleted == 0);
            if (note != null)
            {
                note.IsDeleted = 1;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Note>> GetUserNotesAsync(int userId)
        {
            return await _context.Note
                .Where(n => n.UserId == userId && n.IsDeleted == 0)
                .ToListAsync();
        }
    }
}
