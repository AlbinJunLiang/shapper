using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Shapper.Validators;

public class SingleLevelJsonAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null)
            return ValidationResult.Success;

        if (value is not string jsonString)
            return new ValidationResult("Invalid JSON format.");

        try
        {
            using var document = JsonDocument.Parse(jsonString);

            if (document.RootElement.ValueKind != JsonValueKind.Object)
                return new ValidationResult("JSON must be an object.");

            foreach (var property in document.RootElement.EnumerateObject())
            {
                if (
                    property.Value.ValueKind == JsonValueKind.Object
                    || property.Value.ValueKind == JsonValueKind.Array
                )
                {
                    return new ValidationResult(
                        "JSON must be single-level (no nested objects or arrays)."
                    );
                }
            }

            return ValidationResult.Success;
        }
        catch
        {
            return new ValidationResult("Invalid JSON format.");
        }
    }
}
