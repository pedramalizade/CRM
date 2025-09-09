namespace CRM_Application.Services
{
    public class NoteService : INoteService
    {
        private readonly INoteRepository _noteRepository;

        public NoteService(INoteRepository noteRepository)
        {
            _noteRepository = noteRepository;
        }

        public async Task<List<Note>> GetAllNotesAsync()
        {
            return await _noteRepository.GetAllAsync();
        }

        public async Task<Note?> GetNoteByIdAsync(int id, int userId)
        {
            var note = await _noteRepository.GetByIdAsync(id);
            if (note == null || note.UserId != userId)
                return null;

            return note;
        }

        public async Task AddNoteAsync(Note note, int userId)
        {
            note.UserId = userId;
            note.IsDeleted = 0;
            await _noteRepository.AddAsync(note);
        }

        public async Task UpdateNoteAsync(Note note, int userId)
        {
            var existingNote = await _noteRepository.GetByIdAsync(note.Id);
            if (existingNote == null || existingNote.UserId != userId)
                throw new UnauthorizedAccessException("شما اجازه ویرایش این یادداشت را ندارید.");

            existingNote.Content = note.Content;
            existingNote.CompanyId = note.CompanyId;

            await _noteRepository.UpdateAsync(existingNote);
        }

        public async Task DeleteNoteAsync(int id, int userId)
        {
            var note = await _noteRepository.GetByIdAsync(id);
            if (note == null || note.UserId != userId)
                throw new UnauthorizedAccessException("شما اجازه حذف این یادداشت را ندارید.");

            await _noteRepository.DeleteAsync(id);
        }

        public async Task<List<Note>> GetUserNotesAsync(int userId)
        {
            return await _noteRepository.GetUserNotesAsync(userId);
        }
    }
}
