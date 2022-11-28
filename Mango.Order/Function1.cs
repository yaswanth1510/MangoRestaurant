using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Mango.Order
{
    public class Function1
    {
        private readonly ILogger _logger;

        public Function1(ILogger logger)
        {
            _logger = logger;
        }

        [FunctionName("Function1")]
        public void Run([ServiceBusTrigger("checkoutmessagetopic", "mangoOrderSubscription", Connection = "MangoService")]string mySbMsg)
        {
            if (mySbMsg != null)
            {
                _logger.LogInformation($"C# ServiceBus topic trigger function processed message: {mySbMsg}");
            }
            
        }
    }
}
