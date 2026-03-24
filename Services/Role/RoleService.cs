using AutoMapper;
using Shapper.Dtos;
using Shapper.Models;
using Shapper.Repositories.Roles;

namespace Shapper.Services.Roles
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;

        public RoleService(IRoleRepository roleRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public async Task<List<RoleDto>> GetAllRolesAsync()
        {
            var roles = await _roleRepository.GetAllAsync();
            return _mapper.Map<List<RoleDto>>(roles);
        }

        public async Task<RoleDto?> GetRoleByIdAsync(int id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            return role == null ? null : _mapper.Map<RoleDto>(role);
        }

        public async Task<PagedResponseDto<RoleResponseDto>> GetPaginatedRolesAsync(
            int page,
            int pageSize
        )
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            pageSize = pageSize > 100 ? 100 : pageSize;

            var (roles, totalCount) = await _roleRepository.GetPaginatedAsync(page, pageSize);

            var mapped = _mapper.Map<List<RoleResponseDto>>(roles);

            return new PagedResponseDto<RoleResponseDto>
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                Data = mapped,
            };
        }

        public async Task<RoleResponseDto> CreateRoleAsync(RoleDto roleDto)
        {
            var existingRole = await _roleRepository.GetByNameAsync(roleDto.Name);

            if (existingRole != null)
                throw new InvalidOperationException("Role name already exists.");

            var role = _mapper.Map<Role>(roleDto);

            await _roleRepository.AddAsync(role);

            return _mapper.Map<RoleResponseDto>(role);
        }

        public async Task<RoleResponseDto> UpdateRoleAsync(int id, RoleDto roleDto)
        {
            var existingRole = await _roleRepository.GetByIdAsync(id);
            if (existingRole == null)
                throw new KeyNotFoundException("Role not found");

            // Check if another role already has the new name
            var otherRole = await _roleRepository.GetByNameAsync(roleDto.Name);
            if (otherRole != null && otherRole.Id != id)
                throw new InvalidOperationException("Role name already exists.");

            existingRole.Name = roleDto.Name;
            existingRole.Description = roleDto.Description;

            await _roleRepository.UpdateAsync(existingRole);

            return new RoleResponseDto
            {
                Id = existingRole.Id,
                Name = existingRole.Name,
                Description = existingRole.Description,
            };
        }

        public async Task DeleteRoleAsync(int id)
        {
            var role = await _roleRepository.GetByIdAsync(id);

            if (role == null)
                throw new KeyNotFoundException("Role not found");

            var isUsed = await _roleRepository.RoleIsUsedAsync(id);

            if (isUsed)
                throw new InvalidOperationException("The role cannot be deleted");

            await _roleRepository.DeleteAsync(role);
        }
    }
}
