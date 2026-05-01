using System.Text.Json;
using Shapper.Dtos;

namespace Shapper.Helpers
{
    public static class JsonHelper
    {
        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

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
                return "";
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
                    return "";
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

        public static ExtraDataDto ParseExtraData(string? extraData)
        {
            if (string.IsNullOrWhiteSpace(extraData) || extraData.Trim().ToLower() == "null")
                return new ExtraDataDto();

            try
            {
                return JsonSerializer.Deserialize<ExtraDataDto>(extraData, _options)
                    ?? new ExtraDataDto();
            }
            catch (JsonException)
            {
                return new ExtraDataDto();
            }
        }
    }
}
