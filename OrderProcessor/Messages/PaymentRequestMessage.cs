

using Mango.MessageBus;

namespace DBAcccessProcessor.Messages
{
    public class PaymentRequestMessage : BaseMessage
    {
        public int OrderId { get; set; }
        public string Name { get; set; }
        public string CardNumber { get; set; }
        public string CVV { get; set; }
        public string ExpirationDate { get; set; }
        public double TotalAmount { get; set; }
        public string Email { get; set; }
    }
}
