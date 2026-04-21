namespace Shapper.Dtos.Faqs
{
    public class FaqResponseDto
    {
        public int Id { get; set; }

        public int StoreId { get; set; }


        // Inicializamos con string.Empty para matar los warnings CS8618
        public string Question { get; set; } = string.Empty;

        public string Answer { get; set; } = string.Empty;

        public string Url { get; set; } = string.Empty;

        public int DisplayOrder { get; set; }

        public string Status { get; set; } = "ACTIVE";
    }
}