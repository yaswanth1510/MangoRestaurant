using System.Text;
using Azure.Messaging.ServiceBus;
using Mango.MessageBus;
using Mango.Services.PaymentAPI.Messages;
using Newtonsoft.Json;
using PaymentProcessor;

namespace Mango.Services.PaymentAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly IProcessPayment _processPayment;
        private readonly string ServiceBusConnectionString;
        private readonly string PaymentSubscriber;
        private readonly string PaymentProcessTopic;
        private readonly string OrderUpdatePaymentResultTopic;
        private readonly IConfiguration _configuration;
        private ServiceBusProcessor _orderPaymentProcessor;
        private readonly IMessageBus _messageBus;

        public AzureServiceBusConsumer(IProcessPayment processPayment, IConfiguration configuration, IMessageBus messageBus)
        {
            _processPayment = processPayment;
            _configuration = configuration;
            _messageBus = messageBus;
            ServiceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            PaymentSubscriber = _configuration.GetValue<string>("PaymentSubscriber");
            PaymentProcessTopic = _configuration.GetValue<string>("PaymentProcessTopic");
            OrderUpdatePaymentResultTopic = _configuration.GetValue<string>("OrderUpdatePaymentResultTopic");
            var client = new ServiceBusClient(ServiceBusConnectionString);
            _orderPaymentProcessor = client.CreateProcessor(PaymentProcessTopic, PaymentSubscriber);
        }

        public async Task Start()
        {
            _orderPaymentProcessor.ProcessMessageAsync += ProcessPayments;
            _orderPaymentProcessor.ProcessErrorAsync += ErrorHandler;
            await _orderPaymentProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await _orderPaymentProcessor.StopProcessingAsync();
            await _orderPaymentProcessor.DisposeAsync();
        }

        Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        private async Task ProcessPayments(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            if (message != null)
            {
                var body = Encoding.UTF8.GetString(message.Body);

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
                    await _messageBus.PublishMessage(updatePaymentResultMessage, OrderUpdatePaymentResultTopic);
                    await args.CompleteMessageAsync(args.Message);
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
