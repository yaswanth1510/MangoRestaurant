using AutoMapper;
using Mango.Services.ProductAPI.Domain.Enitities;
using Mango.Services.ProductAPI.Domain.Interface;
using Mango.Services.ProductAPI.Domain.Models.DTO;
using Mango.Services.ProductAPI.Domain.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Mango.Services.ProductAPI.Domain.Handlers
{
    public class GetAllProductsHandler : IRequestHandler<GetAllProductsQuery, ResponseDto>
    {
        private IProductRepository<Product> _productRepository;
        protected ResponseDto responseDto;
        private readonly ILogger<GetAllProductsHandler> _logger;

        public GetAllProductsHandler(IProductRepository<Product> productRepository, ILogger<GetAllProductsHandler> logger)
        {
            _productRepository = productRepository;
            this.responseDto = new ResponseDto();
            _logger = logger;
        }

        public async Task<ResponseDto> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                responseDto.Result = await _productRepository.GetAll();
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
