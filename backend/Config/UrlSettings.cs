namespace Shapper.Config
{
    public class UrlSettings
    {
        public const string SectionName = "PaymentSettings";
        public List<string> AllowedHosts { get; set; } = new();
    }
}
