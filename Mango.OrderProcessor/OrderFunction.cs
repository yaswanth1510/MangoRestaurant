using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using DBAcccessProcessor.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Mango.OrderProcessor
{
    public class OrderFunction
    {
        private readonly ILogger<OrderFunction> _logger;
        private readonly IOrderProcessorCheckout _orderProcessorCheckout;
        public OrderFunction(ILogger<OrderFunction> log, IOrderProcessorCheckout orderProcessorCheckout)
        {
            _logger = log;
            _orderProcessorCheckout = orderProcessorCheckout;
        }

        [FunctionName("OrderFunction")]
        public async Task Run([ServiceBusTrigger("checkoutmessagetopic", "mangoOrderSubscription", Connection = "MangoConnectionString")]string mySbMsg, Int32 deliveryCount,
            DateTime enqueuedTimeUtc,
            string messageId)
        {
            await _orderProcessorCheckout.OnCheckoutMessageReceived(mySbMsg);
            _logger.LogInformation($"C# ServiceBus topic trigger function processed message: {mySbMsg}");
        }
    }
}
