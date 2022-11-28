using System;
using System.Threading.Tasks;
using DBAcccessProcessor.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Mango.EmailProcessor
{
    public class EmailFunction
    {
        private readonly ILogger<EmailFunction> _logger;
        private readonly IOrderPaymentUpdateReceived _orderPaymentUpdate;

        public EmailFunction(ILogger<EmailFunction> logger, IOrderPaymentUpdateReceived orderPaymentUpdate)
        {
            _logger = logger;
            _orderPaymentUpdate = orderPaymentUpdate;
        }

        [FunctionName("EmailFunction")]
        public async Task Run([ServiceBusTrigger("orderupdatepaymentresulttpoic", "emailSubscription", Connection = "MangoConnectionString")]string mySbMsg)
        {
            await _orderPaymentUpdate.OnOrderPaymentUpdateReceived(mySbMsg);
            _logger.LogInformation($"C# ServiceBus topic trigger function processed message: {mySbMsg}");
        }
    }
}
