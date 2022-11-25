using System.Text;
using AutoMapper;
using Azure.Messaging.ServiceBus;
using Mango.MessageBus;
using Mango.Services.OrderAPI.Messages;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Repository;
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
        private readonly string PaymentProcessTopic;
        private readonly string OrderUpdatePaymentResultTopic;
        private readonly IConfiguration _configuration;
        private ServiceBusProcessor checkoutProcessor;
        private ServiceBusProcessor orderUpdatePaymentStatusProcessor;
        private readonly IMessageBus _messageBus;

        public AzureServiceBusConsumer(OrderRepository orderRepository, IMapper mapper, IConfiguration configuration, IMessageBus messageBus)
        {
            _OrderRepository = orderRepository;
            _mapper = mapper;
            _configuration = configuration;
            _messageBus = messageBus;
            ServiceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            CheckoutMessageTopic = _configuration.GetValue<string>("CheckoutMessageTopic");
            SubscriptionName = _configuration.GetValue<string>("SubscriptionName");
            PaymentProcessTopic = _configuration.GetValue<string>("PaymentProcessTopic");
            OrderUpdatePaymentResultTopic = _configuration.GetValue<string>("OrderUpdatePaymentResultTopic");

            var client = new ServiceBusClient(ServiceBusConnectionString);
            checkoutProcessor = client.CreateProcessor(CheckoutMessageTopic, SubscriptionName);
            orderUpdatePaymentStatusProcessor = client.CreateProcessor(OrderUpdatePaymentResultTopic, SubscriptionName);
        }

        public async Task Start()
        {
            checkoutProcessor.ProcessMessageAsync += OnCheckoutMessageReceived;
            checkoutProcessor.ProcessErrorAsync += ErrorHandler;
            await checkoutProcessor.StartProcessingAsync();

            orderUpdatePaymentStatusProcessor.ProcessMessageAsync += OnOrderPaymentUpdateReceived;
            orderUpdatePaymentStatusProcessor.ProcessErrorAsync += ErrorHandler;
            await orderUpdatePaymentStatusProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await checkoutProcessor.StopProcessingAsync();
            await checkoutProcessor.DisposeAsync();

            await orderUpdatePaymentStatusProcessor.StopProcessingAsync();
            await orderUpdatePaymentStatusProcessor.DisposeAsync();
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

                PaymentRequestMessage paymentRequestMessage = new()
                {
                    OrderId = orderHeader.OrderHeaderId,
                    Name = orderHeader.FirstName + " " + orderHeader.LastName,
                    TotalAmount = orderHeader.OrderTotal,
                    CardNumber = orderHeader.CardNumber,
                    CVV = orderHeader.CVV,
                    ExpirationDate = orderHeader.ExpiryMonthYear,
                    Email = orderHeader.Email
                };

                try
                {
                    await _messageBus.PublishMessage(paymentRequestMessage, PaymentProcessTopic);
                    await args.CompleteMessageAsync(args.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        private async Task OnOrderPaymentUpdateReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            if (message != null)
            {
                var body = Encoding.UTF8.GetString(message.Body);

                UpdatePaymentResultMessage paymentResultMessage = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(body);

                await _OrderRepository.UpdateOrderPaymentStatus(paymentResultMessage.OrderId,
                    paymentResultMessage.Status);
                await args.CompleteMessageAsync(args.Message);
            }
        }
    }
}
