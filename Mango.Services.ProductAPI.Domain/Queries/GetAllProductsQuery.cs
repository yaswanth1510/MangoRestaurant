using Mango.Services.ProductAPI.Domain.Models.DTO;
using MediatR;

namespace Mango.Services.ProductAPI.Domain.Queries
{
    public record GetAllProductsQuery : IRequest<ResponseDto>
    {
    }
}
