using Mango.Services.ProductAPI.Domain.Enitities;
using Mango.Services.ProductAPI.Domain.Exceptions;
using Mango.Services.ProductAPI.Domain.Interface;
using Mango.Services.ProductAPI.Domain.Models.DTO;
using Mango.Services.ProductAPI.Domain.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Mango.Services.ProductAPI.Domain.Handlers
{
    public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, ResponseDto>
    {
        private IProductRepository<Product> _productRepository;
        protected ResponseDto responseDto;
        private readonly ILogger<GetProductByIdHandler> _logger;

        public GetProductByIdHandler(IProductRepository<Product> productRepository, ILogger<GetProductByIdHandler> logger)
        {
            _productRepository = productRepository;
            this.responseDto = new ResponseDto();
            _logger = logger;
        }

        public async Task<ResponseDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var product = await _productRepository.GetById(request.id);
                if (product != null)
                {
                    responseDto.Result = product;
                }
                else
                {
                    responseDto.DisplayMessage = $"Product {product.Name} not found";
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message.ToString());
                responseDto.IsSuccess = false;
                responseDto.ErrorMessage = new List<string>() { e.Message.ToString() };
            }

            return responseDto;
        }
    }
}
