using System.Security.Claims; // <-- necesario para Claim, ClaimTypes, ClaimsIdentity, ClaimsPrincipal
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shapper.Dtos;
using Shapper.Models;
using Shapper.Services.Contacts;

namespace Shapper.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactsController : ControllerBase
    {
        private readonly IContactService _contactService;

        public ContactsController(IContactService contactService)
        {
            _contactService = contactService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetContactByUserId(int userId)
        {
            var contact = await _contactService.GetContactByUserIdAsync(userId);

            if (contact == null)
                return NotFound(new { message = "User or contact does not exist" });

            return Ok(contact);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ContactDto contactDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var contact = await _contactService.CreateContactAsync(contactDto);

                if (contact == null)
                    return BadRequest(new { message = "Unable to create contact." });

                return CreatedAtAction(
                    nameof(GetContactByUserId),
                    new { userId = contact.UserId },
                    new { message = "Contact created successfully", data = contact }
                );
            }
            catch (InvalidOperationException ex)
            {
                return ex.Message switch
                {
                    "User does not exist." => NotFound(new { message = ex.Message, status = 404 }),

                    "User already has a contact." => Conflict(
                        new { message = ex.Message, status = 409 }
                    ),

                    "Phone number is already registered." => Conflict(
                        new { message = ex.Message, status = 409 }
                    ),

                    _ => StatusCode(500, new { message = "Internal server error." }),
                };
            }
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> Update(int userId, [FromBody] ContactUpdateDto contactDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updatedContact = await _contactService.UpdateContactAsync(userId, contactDto);

                return Ok(new { message = "Contact updated successfully", data = updatedContact });
            }
            catch (InvalidOperationException ex)
            {
                return ex.Message switch
                {
                    "Contact not found." => NotFound(new { message = ex.Message, status = 404 }),
                    "Phone number is already registered." => Conflict(
                        new { message = ex.Message, status = 409 }
                    ),

                    _ => StatusCode(500, new { message = "Internal server error." }),
                };
            }
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteContactAsync(int userId)
        {
            try
            {
                await _contactService.DeleteContactAsync(userId);
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
