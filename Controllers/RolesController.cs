using System.Security.Claims; // <-- necesario para Claim, ClaimTypes, ClaimsIdentity, ClaimsPrincipal
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shapper.Dtos;
using Shapper.Dtos.Roles;
using Shapper.Models;
using Shapper.Services.Roles;

namespace Shapper.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(RoleDto dto)
        {
            try
            {
                var role = await _roleService.CreateRoleAsync(dto);
                return Ok(role);
            }
            catch (InvalidOperationException ex)
                when (ex.Message.Contains("Role name already exists."))
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPaginationRoles(int page = 1, int pageSize = 10)
        {
            if (page <= 0)
                return BadRequest("Page number must be greater than 0.");

            if (pageSize <= 0 || pageSize > 100)
                return BadRequest("Page size must be between 1 and 100.");
            var result = await _roleService.GetPaginatedRolesAsync(page, pageSize);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoleAsync(int id, RoleDto roleDto)
        {
            if (string.IsNullOrWhiteSpace(roleDto.Name))
                return BadRequest(new { message = "Name is required." });

            try
            {
                var serviceDto = new RoleDto
                {
                    Name = roleDto.Name,
                    Description = roleDto.Description,
                };

                var updatedRole = await _roleService.UpdateRoleAsync(id, serviceDto);

                return Ok(updatedRole); // Retorna el objeto completo con Id
            }
            catch (KeyNotFoundException ex) when (ex.Message.Contains("Role not found"))
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
                when (ex.Message.Contains("Role name already exists."))
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoleAsync(int id)
        {
            try
            {
                await _roleService.DeleteRoleAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}
