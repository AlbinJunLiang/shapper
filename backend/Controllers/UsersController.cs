using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shapper.Dtos.Users;
using Shapper.Helpers;
using Shapper.Services.Users;

namespace Shapper.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public record CreateUserInput(string Name, string? LastName);

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize(Policy = "AdminOnly")]
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


        [Authorize(Policy = "AdminOnly")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();
            return Ok(user);
        }


        [Authorize(Policy = "AdminOnly")]
        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetByEmail(String email)
        {
            var user = await _userService.GetByEmailAsync(email);
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserInput input)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(new { message = "Invalid token: Email claim is missing." });
            }

            bool.TryParse(User.FindFirst("email_verified")?.Value, out bool emailVerified);

            var dto = new CreateUserDto
            {
                Name = input.Name,
                LastName = input.LastName ?? "",
                Email = email,
                Status = emailVerified ? "VERIFIED" : "REGISTERED",
            };

            try
            {
                var (user, isNew) = await _userService.UpsertUserAsync(dto);

                if (isNew)
                {
                    return CreatedAtAction(
                        nameof(Get),
                        new { id = user.Id },
                        new
                        {
                            message = "User created successfully",
                            data = user,
                            isNew = true,
                        }
                    );
                }

                return Ok(
                    new
                    {
                        message = "User session synchronized",
                        data = user,
                        isNew = false,
                    }
                );
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


        [Authorize]
        [HttpPatch("customer/{email}")]
        public async Task<IActionResult> UpdateUser(
            [FromRoute] string email,
            [FromBody] UpdateUserForCustomerDto dto
        )
        {
            var claimEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(claimEmail))
            {
                return Unauthorized(new { message = "Invalid token: Email claim is missing." });
            }

            if (!string.Equals(claimEmail, email, StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            if (dto == null)
                return BadRequest(new { Message = "Invalid data." });

            try
            {
                var serviceDto = new UpdateUserDto
                {
                    Name = dto.Name,
                    LastName = dto.LastName,
                    PhoneNumber = dto.PhoneNumber,
                    Address = dto.Address,
                };

                var updatedUser = await _userService.UpdateUserAsync(claimEmail, serviceDto);

                return Ok(
                    new
                    {
                        updatedUser.Name,
                        updatedUser.LastName,
                        updatedUser.PhoneNumber,
                        updatedUser.Address,
                    }
                );
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { Message = "User not found." });
            }
            catch (InvalidOperationException ex)
            {
                // Esto devolverá un error 400 con el mensaje:
                // "Invalid status: XYZ. Valid statuses are: ACTIVE, REGISTERED..."
                return BadRequest(new { ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = "Internal server error." });
            }
        }

        /*To update for any users in the database*/
        /*Only the admin user can use this endpoint*/


        [Authorize(Policy = "AdminOnly")]
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
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = "Internal server error." });
            }
        }


        [Authorize(Policy = "AdminOnly")]
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

        private List<string> ValidateCreateUserDto(CreateUserDto dto)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.Name))
                errors.Add("Name is required.");

            if (!string.IsNullOrWhiteSpace(dto.LastName) && dto.LastName.Length > 300)
                errors.Add("Last name cannot exceed 300 characters.");

            if (string.IsNullOrWhiteSpace(dto.Email))
                errors.Add("Email is required.");
            else if (!EmailValidationHelper.IsValidEmail(dto.Email))
                errors.Add("Invalid email address.");

            return errors;
        }
    } // Final Controller
} // Final 
