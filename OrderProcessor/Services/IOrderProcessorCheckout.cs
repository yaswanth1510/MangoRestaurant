namespace DBAcccessProcessor.Services
{
    public interface IOrderProcessorCheckout
    {
        Task OnCheckoutMessageReceived(string body);
        Task OnOrderPaymentUpdateReceived(string body);
    }
}
