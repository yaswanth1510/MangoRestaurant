using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mango.Services.ShoppingCartAPI.Domain.Models.Dto;
using MediatR;

namespace Mango.Services.ShoppingCartAPI.Domain.Commands
{
    public record CreateUpdateCartCommand : IRequest<ResponseDto>
    {
        public virtual CartHeaderDto CartHeader { get; set; }
        public virtual IEnumerable<CartDetailsDto> CartDetails { get; set; }
    }
}
