namespace CRM_Domain.Interfaces.Repository
{
    public interface INoteRepository
    {
        Task<List<Note>> GetAllAsync();
        Task<Note?> GetByIdAsync(int id);
        Task AddAsync(Note note);
        Task UpdateAsync(Note note);
        Task DeleteAsync(int id);
        Task<List<Note>> GetUserNotesAsync(int userId);
    }
}
