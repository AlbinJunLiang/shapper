using System.ComponentModel.DataAnnotations;

namespace Shapper.Dtos.Reviews
{
    public class ReviewResponse2Dto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
    }
}
