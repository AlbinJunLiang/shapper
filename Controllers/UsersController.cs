using System.Security.Claims; // <-- necesario para Claim, ClaimTypes, ClaimsIdentity, ClaimsPrincipal
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shapper.Dtos;
using Shapper.Models;
using Shapper.Services.Users;

namespace Shapper.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaginatedUsersAsync(int page = 1, int pageSize = 10)
        {
            if (page <= 0)
                return BadRequest(
                    new { success = false, message = "Page number must be greater than 0." }
                );

            if (pageSize <= 0 || pageSize > 100)
                return BadRequest(
                    new { success = false, message = "Page size must be between 1 and 100." }
                );

            var result = await _userService.GetPaginatedUsersAsync(page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserDto dto)
        {
            try
            {
                var user = await _userService.CreateUserAsync(dto);

                return CreatedAtAction(
                    nameof(Get),
                    new { id = user.Id },
                    new { message = "User created successfully", data = user }
                );
            }
            catch (InvalidOperationException ex)
                when (ex.Message.Contains("Email already registered"))
            {
                return Conflict(new { message = "Email already registered", status = 409 });
            }
            catch (InvalidOperationException ex)
                when (ex.Message.Contains("Default role not configured."))
            {
                return BadRequest(new { message = "Default role not configured", status = 400 });
            }
        }

        //  [Authorize(Policy = "AdminOnly")]
        // [Authorize(Policy = "AdminOnly")]
        [Authorize(Policy = "EmailVerifiedOnly")]
        [HttpGet("endpoint")]
        public IActionResult MyEndpoint()
        {
            var uid = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
            bool emailVerified = bool.Parse(
                HttpContext.User.FindFirst("email_verified")?.Value ?? "false"
            );

            return Ok(
                new
                {
                    Message = "Token válido",
                    UID = uid,
                    Email = email,
                    EmailVerified = emailVerified,
                }
            );
        }

        /*To update only for the client user*/
        [HttpPatch("customer/{email}")]
        public async Task<IActionResult> UpdateUserName(
            [FromRoute] string email,
            [FromBody] UpdateUserForCustomerDto dto
        )
        {
            if (dto == null)
                return BadRequest(new { Message = "Invalid data." });

            try
            {
                var serviceDto = new UpdateUserDto { Name = dto.Name, LastName = dto.LastName };

                var updatedUser = await _userService.UpdateUserAsync(email, serviceDto);

                return Ok(new { updatedUser.Name, updatedUser.LastName });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { Message = "User not found." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = "Internal server error." });
            }
        }

        /*To update for any users in the database*/
        /*Only the admin user can use this endpoint*/
        [HttpPatch("{email}")]
        public async Task<IActionResult> UpdateUser(
            [FromRoute] string email,
            [FromBody] UpdateUserForAdminDto dto
        )
        {
            if (dto == null)
                return BadRequest(new { Message = "Invalid data." });

            try
            {
                var serviceDto = new UpdateUserDto
                {
                    Name = dto.Name,
                    LastName = dto.LastName,
                    RoleId = dto.RoleId,
                    Status = dto.Status,
                };
                var updatedUser = await _userService.UpdateUserAsync(email, serviceDto);

                return Ok(
                    new
                    {
                        updatedUser.Name,
                        updatedUser.LastName,
                        updatedUser.RoleId,
                        updatedUser.Status,
                    }
                );
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { Message = "User not found." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = "Internal server error." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAsync(int id)
        {
            try
            {
                await _userService.DeleteUserAsync(id);
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
    } // Final Controller
} // Final 
