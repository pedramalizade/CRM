namespace CRM_Application.Services
{
    public class BusinessService : IBusinessService
    {
        private readonly IBusinessRepository _repository;

        public BusinessService(IBusinessRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Business>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Business?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<Business> CreateAsync(Business business)
        {
            return await _repository.AddAsync(business);
        }

        public async Task<Business> UpdateAsync(Business business)
        {
            return await _repository.UpdateAsync(business);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _repository.ExistsAsync(id);
        }
    }
}
