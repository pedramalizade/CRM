namespace CRM_Domain.Interfaces.Service
{
    public interface INoteService
    {
        Task<List<Note>> GetAllNotesAsync();
        Task<Note?> GetNoteByIdAsync(int id, int userId);
        Task AddNoteAsync(Note note, int userId);
        Task UpdateNoteAsync(Note note, int userId);
        Task DeleteNoteAsync(int id, int userId);
        Task<List<Note>> GetUserNotesAsync(int userId);
    }
}
