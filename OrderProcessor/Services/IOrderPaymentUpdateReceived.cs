using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAcccessProcessor.Services
{
    public interface IOrderPaymentUpdateReceived
    {
        Task OnOrderPaymentUpdateReceived(string body);
    }
}
