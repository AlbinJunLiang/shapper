using Shapper.Dtos;
using Shapper.Models;

namespace Shapper.Services.Users
{
    public interface IUserService
    {
        Task<UserResponseDto?> GetUserByIdAsync(int id);
        Task<PagedResponseDto<UserResponseDto>> GetPaginatedUsersAsync(int page, int pageSize);

        // DEJA SOLO ESTA LINEA:
        Task<(UserResponseDto User, bool IsNew)> UpsertUserAsync(CreateUserDto createUserDto);
        Task<UserResponseDto> UpdateUserAsync(string email, UpdateUserDto dto);

        Task<UserResponseDto?> GetByEmailAsync(string email);
        Task DeleteUserAsync(int id);
    }
}
