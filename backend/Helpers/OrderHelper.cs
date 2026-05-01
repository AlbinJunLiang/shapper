// Helpers/ValidationHelper.cs
namespace Shapper.Helpers
{
    public static class OrderHelper
    {
        public static string GenerateReference(String prefix)
        {
            var date = DateTime.Now.ToString("yyyyMMdd");
            var random = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();

            return $"{prefix}-{date}-{random}";
        }
    }
}
