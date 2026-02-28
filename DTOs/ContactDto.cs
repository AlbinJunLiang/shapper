using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shapper.Dtos;

public class ContactDto
{
    [StringLength(10, ErrorMessage = "The address cannot exceed 10 characters.")]
    [Required]
    public string Address { get; set; } = string.Empty;

    [StringLength(15, ErrorMessage = "The phone number cannot exceed 15 characters.")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    public int UserId { get; set; }
}

public class ContactUpdateDto
{
    [StringLength(10, ErrorMessage = "The address cannot exceed 10 characters.")]
    [Required]
    public string Address { get; set; } = string.Empty;

    [StringLength(15, ErrorMessage = "The phone number cannot exceed 15 characters.")]
    public string PhoneNumber { get; set; } = string.Empty;
}

public class ContactResponseDto
{
    public int Id { get; set; } // Id del contacto
    public int UserId { get; set; } // Id del usuario al que pertenece
    public string Address { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}
