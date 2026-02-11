using System.Security.Claims; // <-- necesario para Claim, ClaimTypes, ClaimsIdentity, ClaimsPrincipal
using Microsoft.AspNetCore.Mvc;
using Shapper.DTOs;
using Shapper.Middlewares;
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
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
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
        public async Task<IActionResult> Create([FromBody] UserDto userDto)
        {
            var user = await _userService.CreateUserAsync(userDto);
            return CreatedAtAction(nameof(Get), new { id = userDto.Name }, user);
        }

        [HttpGet("endpoint")]
        public IActionResult MyEndpoint()
        {
            // Obtener el UID del usuario desde el token
            var uid = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // También puedes obtener otros claims si los agregaste, por ejemplo email
            var email = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

            if (uid == null)
                return Unauthorized("Token inválido o no proporcionado");

            return Ok(
                new
                {
                    Message = "Token válido",
                    UID = uid,
                    Email = email,
                }
            );
        }
    }
}
