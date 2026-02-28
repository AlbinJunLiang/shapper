using AutoMapper;
using Shapper.Dtos;
using Shapper.Models;
using Shapper.Repositories.Contacts;
using Shapper.Repositories.Users;

namespace Shapper.Services.Contacts
{
    public class ContactService : IContactService
    {
        private readonly IContactRepository _contactRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public ContactService(
            IContactRepository contactRepository,
            IUserRepository userRepository,
            IMapper mapper
        )
        {
            _contactRepository = contactRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<Contact?> GetContactByUserIdAsync(int userId)
        {
            var contact = await _contactRepository.GetByUserIdAsync(userId);
            return contact;
        }

        public async Task<ContactResponseDto?> CreateContactAsync(ContactDto contactDto)
        {
            var userExists = await _userRepository.GetByIdAsync(contactDto.UserId);
            if (userExists == null)
                throw new InvalidOperationException("User does not exist.");

            var existingContact = await _contactRepository.GetByUserIdAsync(contactDto.UserId);
            if (existingContact != null)
                throw new InvalidOperationException("User already has a contact.");

            var phoneExists = await _contactRepository.PhoneExistsAsync(contactDto.PhoneNumber);
            if (phoneExists)
                throw new InvalidOperationException("Phone number is already registered.");

            var contact = _mapper.Map<Contact>(contactDto);

            await _contactRepository.AddAsync(contact);

            return _mapper.Map<ContactResponseDto>(contact);
        }

        public async Task<ContactResponseDto> UpdateContactAsync(int userId, ContactUpdateDto dto)
        {
            var contact = await _contactRepository.GetByUserIdAsync(userId);

            if (contact == null)
                throw new InvalidOperationException("Contact not found.");

            var phoneExists = await _contactRepository.PhoneExistsAsync(dto.PhoneNumber);

            if (phoneExists)
                throw new InvalidOperationException("Phone number is already registered.");

            contact.Address = dto.Address;
            contact.PhoneNumber = dto.PhoneNumber;

            await _contactRepository.UpdateAsync(contact);

            return _mapper.Map<ContactResponseDto>(contact);
        }

        public async Task DeleteContactAsync(int userId)
        {
            var contact = await _contactRepository.GetByUserIdAsync(userId);

            if (contact == null)
                throw new InvalidOperationException("Contact not found.");

            await _contactRepository.DeleteAsync(contact);
        }
    }
}
