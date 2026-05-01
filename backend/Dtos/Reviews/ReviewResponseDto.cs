using System.ComponentModel.DataAnnotations;

namespace Shapper.Dtos.Reviews
{
    public class ReviewResponseDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }

        public int Rating { get; set; }

        // CS8618 Fixed: Comment must contain a non-null value
        public string Comment { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // CS8618 Fixed: Status must contain a non-null value
        public string Status { get; set; } = "ACTIVE";
    }
}
