namespace CRM_Application.Services
{
    public class ContactService : IContactService
    {
        private readonly IContactRepository _contactRepository;

        public ContactService(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }

        public async Task<List<Contact>> GetAllAsync(string? filter = null)
        {
            return await _contactRepository.GetAllAsync(filter);
        }

        public async Task<Contact?> GetByIdAsync(int id)
        {
            return await _contactRepository.GetByIdAsync(id);
        }

        public async Task<Contact> CreateAsync(Contact contact, int userId)
        {
            contact.UserId = userId;
            await _contactRepository.AddAsync(contact);
            return contact;
        }

        public async Task<Contact?> UpdateAsync(int id, Contact updatedContact, int userId)
        {
            var existingContact = await _contactRepository.GetByIdAsync(id);
            if (existingContact == null || existingContact.UserId != userId)
            {
                return null;
            }

            existingContact.Name = updatedContact.Name;
            existingContact.Surname = updatedContact.Surname;
            existingContact.Phone = updatedContact.Phone;
            existingContact.Email = updatedContact.Email;
            existingContact.Position = updatedContact.Position;
            existingContact.CompanyId = updatedContact.CompanyId;

            await _contactRepository.UpdateAsync(existingContact);
            return existingContact;
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var contact = await _contactRepository.GetByIdAsync(id);
            if (contact == null || contact.UserId != userId)
            {
                return false;
            }

            await _contactRepository.SoftDeleteAsync(contact);
            return true;
        }
    }
}
