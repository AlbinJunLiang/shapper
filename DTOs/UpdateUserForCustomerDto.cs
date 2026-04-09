using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shapper.Dtos
{
    public class UpdateUserForCustomerDto
    {
        [MaxLength(300, ErrorMessage = "Name cannot exceed 300 characters")]
        public string? Name { get; set; } // opcional

        [MaxLength(300, ErrorMessage = "Last name cannot exceed 300 characters")]
        public string? LastName { get; set; } // opcional

        [StringLength(300, ErrorMessage = "The address cannot exceed 10 characters.")]
        public string Address { get; set; }

        [StringLength(15, ErrorMessage = "The phone number cannot exceed 15 characters.")]
        public string PhoneNumber { get; set; }
    }
}
