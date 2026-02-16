using Shapper.Services.Payment.Strategies;

namespace Shapper.Services.Payment
{
    public static class PaymentStrategyFactory
    {
        public static IPaymentStrategy Create(string proveedor, string apiServer)
        {
            return proveedor.ToLower() switch
            {
                "stripe" => new StripePayment(apiServer),
                //_ => agregar más proveedores en el futuro
                _ => throw new ArgumentException("Proveedor no soportado")
            };
        }
    }
}
