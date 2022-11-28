using System;
using System.Threading.Tasks;
using DBAcccessProcessor.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Mango.OrderUpdateProcessor
{
    public class OrderUpdateFunction
    {
        private readonly ILogger<OrderUpdateFunction> _logger;
        private readonly IOrderProcessorCheckout _orderProcessorCheckout;

        public OrderUpdateFunction(ILogger<OrderUpdateFunction> logger, IOrderProcessorCheckout orderProcessorCheckout)
        {
            _logger = logger;
            _orderProcessorCheckout = orderProcessorCheckout;
        }

        [FunctionName("OrderUpdateFunction")]
        public async Task Run([ServiceBusTrigger("orderupdatepaymentresulttpoic", "mangoOrderSubscription", Connection = "MangoConnectionString")]string mySbMsg)
        {
            await _orderProcessorCheckout.OnOrderPaymentUpdateReceived(mySbMsg);
            _logger.LogInformation($"C# ServiceBus topic trigger function processed message: {mySbMsg}");
        }
    }
}
