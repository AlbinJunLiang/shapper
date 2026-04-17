using System.Collections.Generic;
using Shapper.Dtos.ProductImages;

namespace Shapper.Dtos.Products
{
    public class ProductStoreView2Dto
    {
        public int Id { get; set; }

        // CS8618 Fixed: Name cannot be null
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public double Price { get; set; } = 0;

        public double Discount { get; set; } = 0;

        public double TaxAmount { get; set; }

        public double NewPrice { get; set; }

        public int Quantity { get; set; } = 0;

        // Ideal para que el frontend no rompa al parsear JSON
        public string Details { get; set; } = "{}";

        public string Status { get; set; } = "ACTIVE";

        public string SubcategoryName { get; set; } = string.Empty;

        // Ya tenías bien la inicialización de la lista, ¡excelente!
        public List<ProductImageDto2> Images { get; set; } = new List<ProductImageDto2>();
    }
}
