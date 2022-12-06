using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Mango.Services.ShoppingCartAPI.Domain.Commands;
using Mango.Services.ShoppingCartAPI.Domain.Entities;
using Mango.Services.ShoppingCartAPI.Domain.Interface;
using Mango.Services.ShoppingCartAPI.Domain.Models.Dto;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Mango.Services.ShoppingCartAPI.Domain.Handlers
{
    public class RemoveCartHandler : IRequestHandler<RemoveCartCommand, ResponseDto>
    {
        private readonly ICartRepository<CartDetails> _cartDetailsRepository;
        private readonly ICartRepository<CartHeader> _cartHeaderRepository;
        private readonly ILogger<ClearCartHandler> _logger;
        private readonly IMapper _mapper;
        protected ResponseDto responseDto;

        public RemoveCartHandler(ICartRepository<CartDetails> cartDetailsRepository, ICartRepository<CartHeader> cartHeaderRepository, ILogger<ClearCartHandler> logger, IMapper mapper)
        {
            _cartDetailsRepository = cartDetailsRepository;
            _cartHeaderRepository = cartHeaderRepository;
            _logger = logger;
            _mapper = mapper;
            this.responseDto = new ResponseDto();
        }

        public async Task<ResponseDto> Handle(RemoveCartCommand request, CancellationToken cancellationToken)
        {
            try
            {
                CartHeader cartHeader = await _cartHeaderRepository.GetById(request.CartDetailsId);
                if (cartHeader is not null)
                {
                    int totalCountOfCartItems = _cartDetailsRepository.Query().Count(x => x.CartHeaderId == cartHeader.CartHeaderId);
                    await _cartHeaderRepository.Remove(cartHeader);
                    if (totalCountOfCartItems == 1)
                    {
                        var cardHeaderToRemove = _cartDetailsRepository.Query().First(x => x.CartHeaderId == cartHeader.CartHeaderId);
                        await _cartDetailsRepository.Remove(cardHeaderToRemove);
                    }
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
