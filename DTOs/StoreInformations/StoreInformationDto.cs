using System.ComponentModel.DataAnnotations;
using Shapper.Validators;

namespace Shapper.Dtos.StoreInformations
{
    public class StoreInformationDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }

        [SingleLevelJson(ErrorMessage = "Details must be a single-level JSON object.")]
        public string Links { get; set; }
        public string CreatedAt { get; set; }
        public int LocationId { get; set; }
        public string AcceptUrl { get; set; }
        public string CancelUrl { get; set; }
    }
}
