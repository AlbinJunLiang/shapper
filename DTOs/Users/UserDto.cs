using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shapper.Dtos.Users
{
    public class CreateUserDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(300, ErrorMessage = "Name cannot exceed 300 characters")]
        public string Name { get; set; }

        [MaxLength(300, ErrorMessage = "Last name cannot exceed 300 characters")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}
