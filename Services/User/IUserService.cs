using Shapper.Dtos;
using Shapper.Models;

namespace Shapper.Services.Users
{
    public interface IUserService
    {
        Task<UserResponseDto?> GetUserByIdAsync(int id);
        Task<PagedResponseDto<UserResponseDto>> GetPaginatedUsersAsync(int page, int pageSize);
        Task<UserResponseDto> CreateUserAsync(CreateUserDto dto);
        Task<UserResponseDto> UpdateUserAsync(string email, UpdateUserDto dto);
        Task<User?> GetByEmailAsync(string email);
        Task DeleteUserAsync(int id);
    }
}
