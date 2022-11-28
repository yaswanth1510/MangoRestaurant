using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using DBAcccessProcessor.Messages;
using DBAcccessProcessor.Repository;
using Newtonsoft.Json;

namespace DBAcccessProcessor.Services
{
    public class OrderPaymentUpdateReceived : IOrderPaymentUpdateReceived
    {
        private readonly IRepository _emailRepository;

        public OrderPaymentUpdateReceived(IRepository emailRepository)
        {
            _emailRepository = emailRepository;
        }

        public async Task OnOrderPaymentUpdateReceived(string body)
        {
            if (body != null)
            {

                UpdatePaymentResultMessage paymentResultMessage = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(body);

                try
                {
                    await _emailRepository.SendAndLogEmail(paymentResultMessage);
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
