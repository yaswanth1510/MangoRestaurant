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
    public class RemoveCouponHandler : IRequestHandler<RemoveCouponCommand, ResponseDto>
    {
        private readonly ICartRepository<CartHeader> _cartHeaderRepository;
        private readonly ILogger<RemoveCouponHandler> _logger;
        protected ResponseDto responseDto;

        public RemoveCouponHandler(ICartRepository<CartHeader> cartHeaderRepository, ILogger<RemoveCouponHandler> logger)
        {
            _cartHeaderRepository = cartHeaderRepository;
            _logger = logger;
            this.responseDto = new ResponseDto();
        }

        public async Task<ResponseDto> Handle(RemoveCouponCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var cartFromDb = _cartHeaderRepository.Query().First(u => u.UserId == request.userId);
                cartFromDb.CouponCode = "";
                var result = await _cartHeaderRepository.Update(cartFromDb);
                if (result.Item2 is true) responseDto.Result = result.Item2;
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
