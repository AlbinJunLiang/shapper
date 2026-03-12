using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shapper.Models
{
    public class StoreInformation
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string StoreImageUrl { get; set; }

        public string AcceptUrl { get; set; }
        public string CancelUrl { get; set; }
        public string Links { get; set; }
        public string CreatedAt { get; set; }

        public int LocationId { get; set; }

        [ForeignKey("LocationId")]
        public Location Location { get; set; }
    }
}
