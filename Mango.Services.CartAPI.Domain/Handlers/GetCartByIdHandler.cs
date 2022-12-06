using AutoMapper;
using Mango.Services.ShoppingCartAPI.Domain.Entities;
using Mango.Services.ShoppingCartAPI.Domain.Interface;
using Mango.Services.ShoppingCartAPI.Domain.Models.Dto;
using Mango.Services.ShoppingCartAPI.Domain.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Mango.Services.ShoppingCartAPI.Domain.Handlers
{
    public class GetCartByIdHandler : IRequestHandler<GetCartByIdQuery, ResponseDto>
    {
        private readonly ICartRepository<CartDetails> _cartDetailsRepository;
        private readonly ICartRepository<CartHeader> _cartHeaderRepository;
        private readonly ICartRepository<Product> _productRepository;
        private readonly IMapper _mapper;
        protected ResponseDto responseDto;
        private readonly ILogger<GetCartByIdHandler> _logger;

        public GetCartByIdHandler(ICartRepository<CartDetails> cartDetailsRepository, ICartRepository<CartHeader> cartHeaderRepository, ICartRepository<Product> productRepository, IMapper mapper, ILogger<GetCartByIdHandler> logger)
        {
            _cartDetailsRepository = cartDetailsRepository;
            _cartHeaderRepository = cartHeaderRepository;
            _mapper = mapper;
            _logger = logger;
            _productRepository = productRepository;
            this.responseDto = new ResponseDto();
        }

        public async Task<ResponseDto> Handle(GetCartByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                Cart cart = new()
                {
                    CartHeader = _cartHeaderRepository.Query().SingleOrDefault(x => x.UserId == request.id),
                };
                if (cart.CartHeader != null)
                {
                    cart.CartDetails = _cartDetailsRepository.Query().Where(x => x.CartHeaderId == cart.CartHeader.CartHeaderId).ToList();
                    foreach (var items in cart.CartDetails)
                    {
                        items.Product = await _productRepository.GetById(items.ProductId);
                    }
                    responseDto.Result = _mapper.Map<CartDto>(cart);
                }
                else
                {
                    responseDto.Result = cart;
                }
            }
            catch (Exception e)
            {
                responseDto.IsSuccess = false;
                responseDto.ErrorMessage = new List<string> { e.Message.ToString() };
                _logger.LogError(e.Message.ToString());
            }

            return responseDto;
        }
    }
}
