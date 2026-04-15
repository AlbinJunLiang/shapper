namespace Shapper.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string OrderReference { get; set; }
        public double Total { get; set; }
        public int? CustomerId { get; set; }
        public string Status { get; set; }
        public string? ExtraData { get; set; } // JSON aquí
        public DateTime CreatedAt { get; set; }

        // Navegacion
        public User? Customer { get; set; } //Puede ser nulo ya hay cliente sin registrar en el sistema
        public ICollection<OrderDetail> OrderDetails { get; set; }
        public ICollection<OrderPayment> OrderPayments { get; set; }
    }
}
