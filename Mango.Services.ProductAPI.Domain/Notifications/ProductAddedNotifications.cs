using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mango.Services.ProductAPI.Domain.Models.DTO;
using MediatR;

namespace Mango.Services.ProductAPI.Domain.Notifications
{
    public record ProductAddedNotifications(ProductDto productDto) : INotification;
}
