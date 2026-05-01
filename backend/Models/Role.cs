using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shapper.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        // Relación 1 a muchos
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
