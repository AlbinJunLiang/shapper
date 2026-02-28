using Shapper.Models;

namespace Shapper.Repositories.Contacts
{
    public interface IContactRepository
    {
        Task<Contact?> GetByUserIdAsync(int userId);

        Task UpdateAsync(Contact contact);

        Task DeleteAsync(Contact contact);
        Task AddAsync(Contact contact);
        Task<bool> PhoneExistsAsync(string phoneNumber);
    }
}
