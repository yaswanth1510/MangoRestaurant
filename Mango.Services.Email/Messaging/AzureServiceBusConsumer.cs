using System.Text;
using Azure.Messaging.ServiceBus;
using Mango.Services.Email.Messages;
using Mango.Services.Email.Repository;
using Newtonsoft.Json;

namespace Mango.Services.Email.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly EmailRepository _emailRepository;
        private readonly string ServiceBusConnectionString;
        private readonly string SubscriptionName;
        private readonly string OrderUpdatePaymentResultTopic;

        private readonly IConfiguration _configuration;
        private ServiceBusProcessor orderUpdatePaymentStatusProcessor;

        public AzureServiceBusConsumer(EmailRepository emailRepository, IConfiguration configuration)
        {
            _emailRepository = emailRepository;
            _configuration = configuration;

            ServiceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            SubscriptionName = _configuration.GetValue<string>("SubscriptionName");
            OrderUpdatePaymentResultTopic = _configuration.GetValue<string>("OrderUpdatePaymentResultTopic");

            var client = new ServiceBusClient(ServiceBusConnectionString);
            orderUpdatePaymentStatusProcessor = client.CreateProcessor(OrderUpdatePaymentResultTopic, SubscriptionName);
        }

        public async Task Start()
        {

            orderUpdatePaymentStatusProcessor.ProcessMessageAsync += OnOrderPaymentUpdateReceived;
            orderUpdatePaymentStatusProcessor.ProcessErrorAsync += ErrorHandler;
            await orderUpdatePaymentStatusProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await orderUpdatePaymentStatusProcessor.StopProcessingAsync();
            await orderUpdatePaymentStatusProcessor.DisposeAsync();
        }

        Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        private async Task OnOrderPaymentUpdateReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            if (message != null)
            {
                var body = Encoding.UTF8.GetString(message.Body);

                UpdatePaymentResultMessage paymentResultMessage = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(body);

                try
                {
                    await _emailRepository.SendAndLogEmail(paymentResultMessage);
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
