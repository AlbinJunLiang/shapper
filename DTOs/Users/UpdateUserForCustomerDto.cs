using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shapper.Dtos.Users
{
    public class UpdateUserForCustomerDto
    {
        [MaxLength(300, ErrorMessage = "Name cannot exceed 300 characters")]
        public string? Name { get; set; } // Opcional, sin warning

        [MaxLength(300, ErrorMessage = "Last name cannot exceed 300 characters")]
        public string? LastName { get; set; } // Opcional, sin warning

        [StringLength(300, ErrorMessage = "The address cannot exceed 300 characters.")]
        // CS8618 Fixed: Inicializado con string.Empty
        public string Address { get; set; } = string.Empty;

        [StringLength(15, ErrorMessage = "The phone number cannot exceed 15 characters.")]
        // CS8618 Fixed: Inicializado con string.Empty
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
