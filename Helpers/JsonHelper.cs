using System.Text.Json;

namespace Shapper.Helpers
{
    public static class JsonHelper
    {
        public static string GetValue(string json, string propertyName)
        {
            try
            {
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    if (doc.RootElement.TryGetProperty(propertyName, out JsonElement element))
                    {
                        return element.ToString();
                    }

                    return GetNestedValue(doc.RootElement, propertyName);
                }
            }
            catch
            {
                return null;
            }
        }

        private static string GetNestedValue(JsonElement element, string path)
        {
            var parts = path.Split('.');
            JsonElement current = element;

            foreach (var part in parts)
            {
                if (
                    current.ValueKind == JsonValueKind.Object
                    && current.TryGetProperty(part, out JsonElement next)
                )
                {
                    current = next;
                }
                else
                {
                    return null;
                }
            }
            return current.ToString();
        }

        public static string? GetString(JsonElement element, string property)
        {
            if (element.TryGetProperty(property, out var value))
            {
                var result = value.GetString();
                return string.IsNullOrEmpty(result) ? null : result;
            }

            return null;
        }

        public static JsonElement? GetObject(JsonElement element, string property)
        {
            if (
                element.TryGetProperty(property, out var value)
                && value.ValueKind == JsonValueKind.Object
            )
            {
                return value;
            }

            return null;
        }
    }
}
