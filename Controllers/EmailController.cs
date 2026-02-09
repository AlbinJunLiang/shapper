using Microsoft.AspNetCore.Mvc;
using Shapper.DTOs;
using Shapper.Services;

namespace Shapper.Controllers
{
    [ApiController]
    [Route("api/email")]
    public class EmailController : ControllerBase
    {
        private readonly EmailService _emailService;

        public EmailController(EmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendEmail(
            [FromQuery] string provider,
            [FromBody] EmailDto email
        )
        {
            if (string.IsNullOrWhiteSpace(provider))
                return BadRequest("El proveedor es requerido");

            await _emailService.SendAsync(provider, email);

            return Ok(new { message = "Correo enviado correctamente" });
        }
    }
}
