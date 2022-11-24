using System.Text;
using AutoMapper;
using Azure.Messaging.ServiceBus;
using Mango.Services.OrderAPI.Messages;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Repository;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Mango.Services.OrderAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly OrderRepository _OrderRepository;
        private readonly IMapper _mapper;
        private readonly string ServiceBusConnectionString;
        private readonly string CheckoutMessageTopic;
        private readonly string SubscriptionName;
        private readonly IConfiguration _configuration;
        private ServiceBusProcessor checkoutProcessor;

        public AzureServiceBusConsumer(OrderRepository orderRepository, IMapper mapper, IConfiguration configuration)
        {
            _OrderRepository = orderRepository;
            _mapper = mapper;
            _configuration = configuration;
            ServiceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            CheckoutMessageTopic = _configuration.GetValue<string>("CheckoutMessageTopic");
            SubscriptionName = _configuration.GetValue<string>("SubscriptionName");

            var client = new ServiceBusClient(ServiceBusConnectionString);
            checkoutProcessor = client.CreateProcessor(CheckoutMessageTopic, SubscriptionName);
        }

        public async Task Start()
        {
            checkoutProcessor.ProcessMessageAsync += OnCheckoutMessageReceived;
            checkoutProcessor.ProcessErrorAsync += ErrorHandler;
            await checkoutProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await checkoutProcessor.StopProcessingAsync();
            await checkoutProcessor.DisposeAsync();
        }

        Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        private async Task OnCheckoutMessageReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            if (message != null)
            {
                var body = Encoding.UTF8.GetString(message.Body);

                CheckoutHeaderDto checkoutHeaderDto = JsonConvert.DeserializeObject<CheckoutHeaderDto>(body);

                var orderHeader = _mapper.Map<OrderHeader>(checkoutHeaderDto);
                orderHeader.OrderTime = DateTime.Now;
                orderHeader.PaymentStatus = false;
                orderHeader.OrderDetails = new List<OrderDetails>();
                foreach (var items in checkoutHeaderDto.CartDetails)
                {
                    OrderDetails orderDetails = _mapper.Map<OrderDetails>(items);
                    orderHeader.CartTotalItems += items.Count;
                    orderHeader.OrderDetails.Add(orderDetails);
                }

                await _OrderRepository.AddOrder(orderHeader);
            }
        }
    }
}
