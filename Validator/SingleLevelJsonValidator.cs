using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Shapper.Validators
{
    public class SingleLevelJsonAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext
        )
        {
            // Si es nulo, dejamos que el atributo [Required] se encargue de la validación si es necesario
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
                    // Verificamos si algún valor es un objeto o un arreglo (anidación)
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
            catch (JsonException)
            {
                return new ValidationResult("Invalid JSON format.");
            }
            catch (Exception)
            {
                return new ValidationResult("An unexpected error occurred during JSON validation.");
            }
        }
    }
}
