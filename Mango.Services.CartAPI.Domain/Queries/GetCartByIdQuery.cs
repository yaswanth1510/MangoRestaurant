using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mango.Services.ShoppingCartAPI.Domain.Models.Dto;
using MediatR;

namespace Mango.Services.ShoppingCartAPI.Domain.Queries
{
    public record GetCartByIdQuery(string id) : IRequest<ResponseDto>;
}
