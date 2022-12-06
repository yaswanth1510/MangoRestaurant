using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mango.Services.ProductAPI.Domain.Notifications;
using MediatR;

namespace Mango.Services.ProductAPI.Domain.Handlers
{
    public class EmailHandler : INotificationHandler<ProductAddedNotifications>
    {
        public async Task Handle(ProductAddedNotifications notification, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }
    }
}
