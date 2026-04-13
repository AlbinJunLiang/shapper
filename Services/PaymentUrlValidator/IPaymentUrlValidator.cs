namespace Shapper.Services.PaymentUrlValidators
{
    public interface IPaymentUrlValidator
    {
        bool IsValidUrl(string? url);
        string GetFullUrl(string path);
    }
}
