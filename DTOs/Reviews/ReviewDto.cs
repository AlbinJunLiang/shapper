using System.ComponentModel.DataAnnotations;

namespace Shapper.Dtos.Reviews
{
    public class ReviewDto
    {
        public int ProductId { get; set; }
        public int UserId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "El rating debe estar entre 1 y 5 estrellas.")]
        public int Rating { get; set; }

        [StringLength(
            300,
            MinimumLength = 3,
            ErrorMessage = "El comentario debe tener entre 3 y 300 caracteres."
        )]
        public string Comment { get; set; }
    }
}
