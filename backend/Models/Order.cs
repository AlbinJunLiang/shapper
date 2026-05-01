namespace Shapper.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string OrderReference { get; set; } = null!;

        public double Total { get; set; }

        public int? CustomerId { get; set; }
        public string Status { get; set; } = "ACTIVE";

        public string? ExtraData { get; set; }

        public DateTime CreatedAt { get; set; }

        public int? LocationId { get; set; }

        public virtual User? Customer { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } =
            new List<OrderDetail>();

        public virtual ICollection<OrderPayment> OrderPayments { get; set; } =
            new List<OrderPayment>();

        public virtual Location? Location { get; set; }
    }
}
