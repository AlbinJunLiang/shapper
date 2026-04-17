using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shapper.Dtos.Subcategories
{
    public class SubcategoryResponse2Dto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public bool Completed { get; set; } = false;
    }
}
