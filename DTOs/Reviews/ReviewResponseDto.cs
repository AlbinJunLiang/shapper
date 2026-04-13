using System.ComponentModel.DataAnnotations;

namespace Shapper.Dtos.Reviews
{
    public class ReviewResponseDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
    }
}
