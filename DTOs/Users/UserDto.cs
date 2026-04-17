using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shapper.Dtos.Users
{
    public class CreateUserDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(300, ErrorMessage = "Name cannot exceed 300 characters")]
        // CS8618 Fixed: Inicializado para evitar el warning de nulabilidad
        public string Name { get; set; } = string.Empty;

        [MaxLength(300, ErrorMessage = "Last name cannot exceed 300 characters")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;

        // Ya tenías este correctamente inicializado
        public string Status { get; set; } = "ACTIVE";
    }
}