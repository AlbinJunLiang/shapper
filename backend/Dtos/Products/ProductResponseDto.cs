using System.Collections.Generic;
using Shapper.Dtos.ProductImages;

namespace Shapper.Dtos.Products
{
    public class ProductResponseDto
    {
        public int Id { get; set; }

        // CS8618 Fixed: Name cannot be null
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public double Price { get; set; } = 0;

        public double TaxAmount { get; set; }

        public int Quantity { get; set; } = 0;

        public double Discount { get; set; } = 0;

        // Inicializado como objeto JSON vacío
        public string Details { get; set; } = "{}";

        public string Status { get; set; } = "ACTIVE";

        public int SubcategoryId { get; set; } // FK clara

        // Ya estaba correctamente inicializado, ¡bien ahí!
        public List<ProductImageDto> Images { get; set; } = new List<ProductImageDto>();
    }
}
