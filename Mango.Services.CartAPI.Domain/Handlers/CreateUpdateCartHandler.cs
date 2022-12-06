using AutoMapper;
using Mango.Services.ShoppingCartAPI.Domain.Commands;
using Mango.Services.ShoppingCartAPI.Domain.Entities;
using Mango.Services.ShoppingCartAPI.Domain.Interface;
using Mango.Services.ShoppingCartAPI.Domain.Models.Dto;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Mango.Services.ShoppingCartAPI.Domain.Handlers
{
    public class CreateUpdateCartHandler : IRequestHandler<CreateUpdateCartCommand, ResponseDto>
    {
        private readonly ICartRepository<CartDetails> _cartDetailsRepository;
        private readonly ICartRepository<CartHeader> _cartHeaderRepository;
        private readonly ICartRepository<Product> _productRepository;
        private readonly ILogger<CreateUpdateCartHandler> _logger;
        private readonly IMapper _mapper;
        protected ResponseDto responseDto;

        public CreateUpdateCartHandler(IMapper mapper, ICartRepository<CartDetails> cartDetailsRepository, ICartRepository<CartHeader> cartHeaderRepository, ICartRepository<Product> productRepository, ILogger<CreateUpdateCartHandler> logger)
        {
            _mapper = mapper;
            _cartDetailsRepository = cartDetailsRepository;
            _cartHeaderRepository = cartHeaderRepository;
            _productRepository = productRepository;
            _logger = logger;
            this.responseDto = new ResponseDto();
        }

        public async Task<ResponseDto> Handle(CreateUpdateCartCommand request, CancellationToken cancellationToken)
        {
            var cart = new Cart()
            {
                CartHeader = _mapper.Map<CartHeaderDto, CartHeader>(request.CartHeader),
            };
            var cartDetails = new List<CartDetails>();
            foreach (var items in request.CartDetails)
            {
                var cartDetail =  _mapper.Map<CartDetailsDto, CartDetails>(items);
                cartDetail.Product = _mapper.Map<ProductDto, Product>(items.Product);
                cartDetails.Add(cartDetail);
            }

            cart.CartDetails = cartDetails;

            try
            {
                var productInDb = await _productRepository.GetById(cart.CartDetails.First().ProductId);
                if (productInDb is null)
                {
                    var details = await _productRepository.Create(cart.CartDetails.First().Product);
                    if (details.Item2 is false) throw new Exception();
                }

                var cartHeaderFromDb =
                            _cartHeaderRepository.Query().FirstOrDefault(x => x.UserId == cart.CartHeader.UserId);
                if (cartHeaderFromDb is null)
                {
                    var result = await _cartHeaderRepository.Create(cart.CartHeader);
                    if (result.Item2 is true)
                    {
                        cart.CartDetails.First().CartHeaderId = cart.CartHeader.CartHeaderId;
                        cart.CartDetails.First().Product = null;
                        var result1 = await _cartDetailsRepository.Create(cart.CartDetails.First());
                        if (result1.Item2 is false) throw new Exception();
                    }
                }
                else
                {
                    var cartDetailsFromDb = _cartDetailsRepository.Query().FirstOrDefault(u => u.ProductId == cart.CartDetails.First().ProductId && u.CartHeaderId == cartHeaderFromDb.CartHeaderId);
                    if (cartDetailsFromDb == null)
                    {
                        cart.CartDetails.First().CartHeaderId = cartHeaderFromDb.CartHeaderId;
                        cart.CartDetails.First().Product = null;
                        var result1 = await _cartDetailsRepository.Create(cart.CartDetails.First());
                        if (result1.Item2 is false) throw new Exception();
                    }
                    else
                    {
                        cart.CartDetails.First().Product = null;
                        cart.CartDetails.First().Count += cartDetailsFromDb.Count;
                        var result1 = await _cartDetailsRepository.Update(cart.CartDetails.First());
                        if (result1.Item2 is false) throw new Exception();
                    }
                }

                responseDto.Result = _mapper.Map<CartDto>(cart);
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
