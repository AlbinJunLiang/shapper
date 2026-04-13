using AutoMapper;
using Shapper.Dtos;
using Shapper.Dtos.Users;
using Shapper.Models;
using Shapper.Repositories.Roles;
using Shapper.Repositories.Users;

namespace Shapper.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;

        public UserService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IMapper mapper
        )
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public async Task<UserResponseDto?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user == null ? null : _mapper.Map<UserResponseDto>(user);
        }

        public async Task<UserResponseDto?> GetByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            return user == null ? null : _mapper.Map<UserResponseDto>(user);
        }

        public async Task<PagedResponseDto<UserResponseDto>> GetPaginatedUsersAsync(
            int page,
            int pageSize
        )
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            pageSize = pageSize > 100 ? 100 : pageSize;

            var (users, totalCount) = await _userRepository.GetPaginatedAsync(page, pageSize);

            var mapped = _mapper.Map<List<UserResponseDto>>(users);

            return new PagedResponseDto<UserResponseDto>
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                Data = mapped,
            };
        }

        public async Task<(UserResponseDto User, bool IsNew)> UpsertUserAsync(
            CreateUserDto createUserDto
        )
        {
            // 1. Buscar si el usuario ya existe por email
            var existingUser = await _userRepository.GetByEmailAsync(createUserDto.Email);

            if (existingUser != null)
            {
                // 2. Lógica de "Despertar" o Actualizar Status:
                // Si el usuario existe pero el status que viene es diferente (ej. pasó de "REGISTERED" a "VERIFIED")
                if (existingUser.Status != createUserDto.Status)
                {
                    existingUser.Status = createUserDto.Status;

                    // También podrías actualizar Nombre/Apellido si detectas cambios
                    existingUser.Name = createUserDto.Name;
                    existingUser.LastName = createUserDto.LastName;

                    await _userRepository.UpdateAsync(existingUser);
                }

                return (_mapper.Map<UserResponseDto>(existingUser), false);
            }

            // 3. Si no existe, creación normal
            var user = _mapper.Map<User>(createUserDto);
            var defaultRole = await _roleRepository.GetByNameAsync("Customer");

            if (defaultRole == null)
                throw new InvalidOperationException("Default role not configured.");

            user.RoleId = defaultRole.Id;

            await _userRepository.AddAsync(user);

            return (_mapper.Map<UserResponseDto>(user), true);
        }

        public async Task<UserResponseDto> UpdateUserAsync(string email, UpdateUserDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            if (!string.IsNullOrEmpty(dto.Name))
                user.Name = dto.Name;

            if (!string.IsNullOrEmpty(dto.LastName))
                user.LastName = dto.LastName;

            if (!string.IsNullOrEmpty(dto.PhoneNumber))
                user.PhoneNumber = dto.PhoneNumber;

            if (!string.IsNullOrEmpty(dto.Address))
                user.Address = dto.Address;

            if (dto.RoleId > 0)
                user.RoleId = dto.RoleId;

            if (!string.IsNullOrEmpty(dto.Status))
                user.Status = dto.Status;

            await _userRepository.UpdateAsync(user);
            return _mapper.Map<UserResponseDto>(user);
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                throw new KeyNotFoundException("User not found");

            await _userRepository.DeleteAsync(user);
        }
    }
}
