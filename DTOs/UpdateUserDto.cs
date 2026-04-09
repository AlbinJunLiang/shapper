using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shapper.Dtos
{

    public class UpdateUserDto
    {
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string? PasswordHash { get; set; }
        public string? Email { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string? Status { get; set; }
        public int RoleId { get; set; }
    }
}
