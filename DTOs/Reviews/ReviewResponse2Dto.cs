using System;
using System.ComponentModel.DataAnnotations;

namespace Shapper.Dtos.Reviews
{
    public class ReviewResponse2Dto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }

        // CS8618 Fixed: Name must contain a non-null value
        public string Name { get; set; } = string.Empty;

        // CS8618 Fixed: LastName must contain a non-null value
        public string LastName { get; set; } = string.Empty;

        public int Rating { get; set; }

        // CS8618 Fixed: Comment must contain a non-null value
        public string Comment { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // CS8618 Fixed: Status must contain a non-null value
        public string Status { get; set; } = "ACTIVE";
    }
}