using Azure.Messaging.ServiceBus;
using DBAcccessProcessor.Messages;
using DBAcccessProcessor.Models;
using DBAcccessProcessor.Repository;
using Mango.MessageBus;
using Newtonsoft.Json;
using System.Text;

namespace DBAcccessProcessor.Services
{
    public class OrderProcessorCheckout : IOrderProcessorCheckout
    {
        private readonly IMessageBus _messageBus;
        private readonly IRepository _orderRepository;

        public OrderProcessorCheckout(IMessageBus messageBus, IRepository orderRepository)
        {
            _messageBus = messageBus;
            _orderRepository = orderRepository;
        }

        public async Task OnCheckoutMessageReceived(string body)
        {

            CheckoutHeaderDto checkoutHeaderDto = JsonConvert.DeserializeObject<CheckoutHeaderDto>(body);
            if (checkoutHeaderDto != null && checkoutHeaderDto.CartDetails != null)
            {
                OrderHeader orderHeader = new OrderHeader()
                {
                    UserId = checkoutHeaderDto.UserId,
                    CouponCode = checkoutHeaderDto.CouponCode,
                    OrderTotal = checkoutHeaderDto.OrderTotal,
                    DiscountTotal = checkoutHeaderDto.DiscountTotal,
                    FirstName = checkoutHeaderDto.FirstName,
                    LastName = checkoutHeaderDto.LastName,
                    PickupDateTime = checkoutHeaderDto.PickupDateTime,
                    Phone = checkoutHeaderDto.Phone,
                    Email = checkoutHeaderDto.Email,
                    CardNumber = checkoutHeaderDto.CardNumber,
                    CVV = checkoutHeaderDto.CVV,
                    ExpiryMonthYear = checkoutHeaderDto.ExpiryMonthYear,
                    CartTotalItems = checkoutHeaderDto.CartTotalItems,
                    OrderDetails = new List<OrderDetails>(),
                    PaymentStatus = false,
                    OrderTime = DateTime.Now
                };
                foreach (var items in checkoutHeaderDto.CartDetails)
                {
                    OrderDetails orderDetails = new OrderDetails()
                    {
                        ProductId = items.ProductId,
                        Count = items.Count,
                        ProductName = items.Product.Name,
                        Price = items.Product.Price,
                    };
                    orderHeader.CartTotalItems += items.Count;
                    orderHeader.OrderDetails.Add(orderDetails);
                }

                try
                {
                    var result = await _orderRepository.AddOrder(orderHeader);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                
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
                    await _messageBus.PublishMessage(paymentRequestMessage, "orderpaymentprocesstopic");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        public async Task OnOrderPaymentUpdateReceived(string body)
        {
           
            if (body != null)
            {
                UpdatePaymentResultMessage paymentResultMessage = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(body);

                await _orderRepository.UpdateOrderPaymentStatus(paymentResultMessage.OrderId,
                    paymentResultMessage.Status);
            }
        }
    }
}