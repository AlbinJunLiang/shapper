using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shapper.Dtos.Categories
{
    public class CategoriesWithGlobalPriceRangeDto
    {
        public double? GlobalMinPrice { get; set; }

        public double? GlobalMaxPrice { get; set; }
        public List<CategoryWithSubcategoriesDto> Categories { get; set; } =
            new List<CategoryWithSubcategoriesDto>();
    }
}
