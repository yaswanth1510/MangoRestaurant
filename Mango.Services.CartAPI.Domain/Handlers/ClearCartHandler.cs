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
    public class ClearCartHandler : IRequestHandler<ClearCartCommand, ResponseDto>
    {
        private readonly ICartRepository<CartDetails> _cartDetailsRepository;
        private readonly ICartRepository<CartHeader> _cartHeaderRepository;
        private readonly ILogger<ClearCartHandler> _logger;
        private readonly IMapper _mapper;
        protected ResponseDto responseDto;

        public ClearCartHandler(ICartRepository<CartDetails> cartDetailsRepository, ICartRepository<CartHeader> cartHeaderRepository, ILogger<ClearCartHandler> logger, IMapper mapper)
        {
            _cartDetailsRepository = cartDetailsRepository;
            _cartHeaderRepository = cartHeaderRepository;
            _logger = logger;
            _mapper = mapper;
            this.responseDto = new ResponseDto();
        }

        public async Task<ResponseDto> Handle(ClearCartCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var cartHeaderFromDb = _cartHeaderRepository.Query().FirstOrDefault(x => x.UserId == request.userId);
                if (cartHeaderFromDb != null)
                {
                    var cartDetailsDb = _cartDetailsRepository.Query()
                        .Where(x => x.CartHeaderId == cartHeaderFromDb.CartHeaderId).ToList();
                    await _cartDetailsRepository.Clear(cartDetailsDb);
                    var result = await _cartHeaderRepository.Remove(cartHeaderFromDb);
                    if (result is true) responseDto.Result = result;
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
