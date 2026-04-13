using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shapper.Dtos.Orders
{
    public class CreateOrderDto
    {
        public int? CustomerId { get; set; }
        public string? ExtraData { get; set; }
        public string Provider { get; set; }
        public string CompanyName { get; set; }
        public string SuccessUrl { get; set; } = "";
        public string CancelUrl { get; set; } = "";

        [Required]
        [MinLength(1)]
        public List<CreateOrderItemDto> Items { get; set; } = new();
    }
}
