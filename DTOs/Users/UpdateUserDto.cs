using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shapper.Dtos.Users
{
    public class UpdateUserDto
    {
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string? PasswordHash { get; set; }
        public string? Email { get; set; }

        // CS8618 Fixed: Address cannot be null
        public string Address { get; set; } = string.Empty;

        // CS8618 Fixed: PhoneNumber cannot be null
        public string PhoneNumber { get; set; } = string.Empty;

        public string? Status { get; set; }
        public int RoleId { get; set; }
    }
}