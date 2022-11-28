using System;
using System.Threading.Tasks;
using Mango.MessageBus;
using Mango.PaymentProcessor.Messages;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PaymentProcessor;

namespace Mango.PaymentProcessor
{
    public class PaymentFunction
    {
        private readonly ILogger<PaymentFunction> _logger;
        private readonly IMessageBus _messageBus;
        private readonly IProcessPayment _processPayment;
        public PaymentFunction(ILogger<PaymentFunction> log, IMessageBus messageBus, IProcessPayment processPayment)
        {
            _logger = log;
            _messageBus = messageBus;
            _processPayment = processPayment;
        }

        [FunctionName("PaymentFunction")]
        public async Task Run([ServiceBusTrigger("orderpaymentprocesstopic", "mangoPayment", Connection = "MangoConnectionString")] string mySbMsg)
        {
            await ProcessPayments(mySbMsg);
            _logger.LogInformation($"C# ServiceBus topic trigger function processed message: {mySbMsg}");
        }

        private async Task ProcessPayments(string body)
        {
            if (body != null)
            {

                PaymentRequestMessage paymentRequestMessage = JsonConvert.DeserializeObject<PaymentRequestMessage>(body);

                var result = _processPayment.PaymentProcessor();

                UpdatePaymentResultMessage updatePaymentResultMessage = new()
                {
                    Status = result,
                    OrderId = paymentRequestMessage.OrderId,
                    Email = paymentRequestMessage.Email
                };

                try
                {
                    await _messageBus.PublishMessage(updatePaymentResultMessage, "orderupdatepaymentresulttpoic");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
    }
}
