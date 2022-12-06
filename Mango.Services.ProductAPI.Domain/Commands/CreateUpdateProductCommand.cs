using Mango.Services.ProductAPI.Domain.Models.DTO;
using MediatR;

namespace Mango.Services.ProductAPI.Domain.Commands
{
    public record CreateUpdateProductCommand : IRequest<ResponseDto>
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public string ImageUrl { get; set; }
    }
}
