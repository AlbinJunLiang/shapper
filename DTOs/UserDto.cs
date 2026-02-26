using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shapper.Dtos
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
    }

    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public int RoleId { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
    }

    public class UpdateUserForCustomerDto
    {
        [MaxLength(300, ErrorMessage = "Name cannot exceed 300 characters")]
        public string? Name { get; set; } // opcional

        [MaxLength(300, ErrorMessage = "Last name cannot exceed 300 characters")]
        public string? LastName { get; set; } // opcional
    }

    public class UpdateUserForAdminDto
    {
        [MaxLength(300, ErrorMessage = "Name cannot exceed 300 characters")]
        public string? Name { get; set; } // opcional

        [MaxLength(300, ErrorMessage = "Last name cannot exceed 300 characters")]
        public string? LastName { get; set; } // opcional

        public int RoleId { get; set; }

        [MaxLength(30, ErrorMessage = "Status cannot exceed 30 characters")]
        public string Status { get; set; }
    }

    public class UpdateUserDto
    {
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string? PasswordHash { get; set; }
        public string? Email { get; set; }
        public string? Status { get; set; }
        public int RoleId { get; set; }
        // otros campos que quieras actualizar internamente
    }
}
