using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mango.Services.ShoppingCartAPI.Domain.Commands;
using Mango.Services.ShoppingCartAPI.Domain.Messages;

namespace Mango.Services.ShoppingCartAPI.Domain.Notifications
{
    public record CheckoutNotification(CheckoutHeaderDto CheckoutHeaderDto) : INotification;
}
