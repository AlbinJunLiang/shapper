using Shapper.Dtos.Locations;
using Shapper.Dtos.StoreLinks;

namespace Shapper.Dtos.StoreInformations
{
    public class StoreInformationResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int? LocationId { get; set; }

        // Información de ubicación (puede ser null)
        public LocationResponseDto? Location { get; set; }

        // Links asociados
        public List<StoreLinkResponseDto> StoreLinks { get; set; } = new();
    }
}
