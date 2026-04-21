namespace Shapper.Dtos.Faqs
{
    public class FaqDto
    {
        public int StoreId { get; set; }

        public string Question { get; set; } = "";
        public string Answer { get; set; } = "";
        public string Url { get; set; } = "";
        public int? DisplayOrder { get; set; }
        public string Status { get; set; } = "";
    }
}
