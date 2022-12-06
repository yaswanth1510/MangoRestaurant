using Mango.Services.ProductAPI.Domain.Commands;
using Mango.Services.ProductAPI.Domain.Enitities;
using Mango.Services.ProductAPI.Domain.Interface;
using Mango.Services.ProductAPI.Domain.Models.DTO;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Mango.Services.ProductAPI.Domain.Handlers
{
    public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, ResponseDto>
    {
        private IProductRepository<Product> _productRepository;
        protected ResponseDto responseDto;
        private readonly ILogger<DeleteProductHandler> _logger;

        public DeleteProductHandler(IProductRepository<Product> productRepository, ILogger<DeleteProductHandler> logger)
        {
            _productRepository = productRepository;
            this.responseDto = new ResponseDto();
            _logger = logger;
        }

        public async Task<ResponseDto> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var product = await _productRepository.GetById(request.id);
                if (product != null)
                {
                    var result = await _productRepository.Delete(product);
                    responseDto.Result = result;
                }
                else
                {
                    responseDto.DisplayMessage = $"Product {product.Name} not yet deleted";
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
