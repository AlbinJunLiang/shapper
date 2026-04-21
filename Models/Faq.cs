using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shapper.Models
{
    public class Faq
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "StoreId is required")]
        public int StoreId { get; set; }

        // PROPIEDAD DE NAVEGACIÓN
        // Usamos '?' para permitir que sea nula al crear el objeto, 
        // pero EF sabrá que la FK es obligatoria por el 'int StoreId' (no nuleable)
        [ForeignKey(nameof(StoreId))]
        public virtual Store? Store { get; set; }

        public string? Question { get; set; }
        public string? Answer { get; set; }
        public string? Url { get; set; }
        public int? DisplayOrder { get; set; }
        public string? Status { get; set; } = "ACTIVE";
    }
}
