using Shapper.Dtos;
using Shapper.Models;

namespace Shapper.Services.Contacts
{
    public interface IContactService
    {
        Task<Contact?> GetContactByUserIdAsync(int userId);

        Task<ContactResponseDto?> CreateContactAsync(ContactDto contactDto);

        Task<ContactResponseDto> UpdateContactAsync(int userId, ContactUpdateDto contactUpdateDto);

        Task DeleteContactAsync(int userId);
    }
}
