using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Shapper.Dtos.Subcategories;

namespace Shapper.Dtos.Categories
{
    public class CategoryWithSubcategoriesDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Completed { get; set; } = false;
        public List<SubcategoryResponse2Dto> Subcategories { get; set; }
    }
}
