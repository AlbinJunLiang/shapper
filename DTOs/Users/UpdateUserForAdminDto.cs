using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shapper.Dtos.Users
{
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
}
