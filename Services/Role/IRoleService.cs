using Shapper.Dtos;
using Shapper.Models;

namespace Shapper.Services.Roles
{
    public interface IRoleService
    {
        Task<List<RoleDto>> GetAllRolesAsync();
        Task<RoleDto?> GetRoleByIdAsync(int id);
        Task<PagedResponseDto<RoleResponseDto>> GetPaginatedRolesAsync(int page, int pageSize);

        Task<RoleResponseDto> CreateRoleAsync(RoleDto roleDto);
        Task<RoleResponseDto> UpdateRoleAsync(int id, RoleDto roleDto);
        Task DeleteRoleAsync(int id);
    }
}
