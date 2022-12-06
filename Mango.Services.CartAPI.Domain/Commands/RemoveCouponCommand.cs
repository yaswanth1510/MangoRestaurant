using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mango.Services.ShoppingCartAPI.Domain.Models.Dto;

namespace Mango.Services.ShoppingCartAPI.Domain.Commands
{
    public record RemoveCouponCommand(string userId) : IRequest<ResponseDto>;
}
