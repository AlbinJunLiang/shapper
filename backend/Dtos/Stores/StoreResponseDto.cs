using Shapper.Dtos.Locations;
using Shapper.Dtos.StoreLinks;

namespace Shapper.Dtos.Store
{
    public class StoreResponseDto
    {
        public int Id { get; set; }
        public string StoreCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        public string MainLocation { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // Información de ubicación (puede ser null)

        // Links asociados
        public List<StoreLinkResponseDto> StoreLinks { get; set; } = new();
    }
}
