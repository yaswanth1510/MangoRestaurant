using AutoMapper;
using Mango.Services.ProductAPI.Domain.Commands;
using Mango.Services.ProductAPI.Domain.Enitities;
using Mango.Services.ProductAPI.Domain.Interface;
using Mango.Services.ProductAPI.Domain.Models.DTO;
using Mango.Services.ProductAPI.Domain.Notifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Mango.Services.ProductAPI.Domain.Handlers
{
    public class CreateUpdateProductHandler : IRequestHandler<CreateUpdateProductCommand, ResponseDto>
    {
        private IProductRepository<Product> _productRepository;
        private IMapper _mapper;
        protected ResponseDto responseDto;
        private readonly ILogger<CreateUpdateProductHandler> _logger;
        private readonly IPublisher _publisher;

        public CreateUpdateProductHandler(IProductRepository<Product> productRepository, IMapper mapper, ILogger<CreateUpdateProductHandler> logger, IPublisher publisher)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _logger = logger;
            this.responseDto = new ResponseDto();
            _publisher = publisher;
        }

        public async Task<ResponseDto> Handle(CreateUpdateProductCommand request, CancellationToken cancellationToken)
        {
            var productDTO = new ProductDto()
            {
                ProductId = request.ProductId,
                Name = request.Name,
                CategoryName = request.CategoryName,
                Description = request.Description,
                Price = request.Price,
                ImageUrl = request.ImageUrl
            };
            Product product = _mapper.Map<ProductDto, Product>(productDTO);
            if (product.ProductId > 0)
            {
                try
                {
                    var result = await _productRepository.Update(product);
                    if (result.Item2 is true)
                    {
                        productDTO = _mapper.Map<Product, ProductDto>(result.Item1);
                        responseDto.Result = productDTO;
                    }
                    else
                    {
                        responseDto.DisplayMessage = $"Product {result.Item1.Name} not yet Updated";
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message.ToString());
                    responseDto.IsSuccess = false;
                    responseDto.ErrorMessage = new List<string>() { e.Message.ToString() };
                }
            }
            else
            {
                try
                {
                    var result = await _productRepository.Create(product);
                    if (result.Item2 is true)
                    {
                        productDTO = _mapper.Map<Product, ProductDto>(result.Item1);
                        responseDto.Result = productDTO;
                    }
                    else
                    {
                        responseDto.DisplayMessage = $"Product {result.Item1.Name} not yet Added";
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message.ToString());
                    responseDto.IsSuccess = false;
                    responseDto.ErrorMessage = new List<string>() { e.Message.ToString() };
                }
            }

            await _publisher.Publish(new ProductAddedNotifications(productDTO));
            return responseDto;
        }
    }
}
