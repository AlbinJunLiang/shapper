using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Shapper.Enums;
using Shapper.Validators;

namespace Shapper.Dtos.Products
{
    public class ProductDto
    {
        [Required(ErrorMessage = "The Name is required.")]
        [MaxLength(150, ErrorMessage = "The Name cannot exceed 150 characters.")]
        public string Name { get; set; } = string.Empty; // CS8618 Fixed

        [MaxLength(500, ErrorMessage = "The Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price must be greater or equal to 0.")]
        public double Price { get; set; } = 0;

        [Range(0, 100, ErrorMessage = "Tax amount must be between 0 and 100.")]
        public double TaxAmount { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be greater or equal to 0.")]
        public int Quantity { get; set; } = 0;

        [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100.")]
        public double Discount { get; set; } = 0;

        [MaxLength(1000, ErrorMessage = "Details cannot exceed 1000 characters.")]
        [SingleLevelJson(ErrorMessage = "Details must be a single-level JSON object.")]
        public string Details { get; set; } = "{}"; // Inicializado como JSON vacío para evitar errores

        [Required(ErrorMessage = "Status is required.")]
        [EnumDataType(
            typeof(ProductStatus),
            ErrorMessage = "Status must be: ACTIVE, INACTIVE, or DISCONTINUED."
        )]
        public string Status { get; set; } = "ACTIVE";

        [Required(ErrorMessage = "Subcategory is required.")]
        public int SubcategoryId { get; set; }
    }
}