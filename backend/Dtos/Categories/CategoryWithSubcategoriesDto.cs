using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Shapper.Dtos.Subcategories;

namespace Shapper.Dtos.Categories
{
    public class CategoryWithSubcategoriesDto
    {
        public int Id { get; set; }

        // Protegemos el string para evitar advertencias en el mapeo
        public string Name { get; set; } = string.Empty;

        public bool Completed { get; set; } = false;

        // Inicializamos la lista. Esto es CRUCIAL para que Angular
        // no reciba un 'null' y rompa el *ngFor
        public List<SubcategoryResponse2Dto> Subcategories { get; set; } =
            new List<SubcategoryResponse2Dto>();
    }
}
