using System;
using System.Collections.Generic;
using Shapper.Dtos.OrderDetails;

namespace Shapper.Dtos.Orders
{
    public class OrderResponseDto
    {
        public int Id { get; set; }

        // CS8618 Fixed: Non-nullable property 'OrderReference' must contain a non-null value
        public string OrderReference { get; set; } = string.Empty;

        public double Total { get; set; }
        public double TotalDiscount { get; set; }
        public double TotalTax { get; set; }
        public double Subtotal { get; set; }
        public double ShippingCost { get; set; } = 0;

        public int? CustomerId { get; set; }

        // CS8618 Fixed: Initialized with default status
        public string Status { get; set; } = "PENDING";

        // ExtraData es opcional (?), así que no necesita inicialización obligatoria,
        // pero tenerlo como null es correcto aquí.
        public ExtraDataDto? ExtraData { get; set; } 

        public int? LocationId { get; set; }

        // CS8618 Fixed: CompanyName cannot be null
        public string CompanyName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Ya tenías esto bien, usando la sintaxis simplificada de C#
        public List<OrderDetailResponseDto> Details { get; set; } = new();
    }
}