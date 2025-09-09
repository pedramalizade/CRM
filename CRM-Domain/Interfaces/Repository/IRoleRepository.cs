namespace CRM_Domain.Interfaces.Repository
{
    public interface IRoleRepository
    {
        Task<Role?> GetByIdAsync(int id);
    }
}
