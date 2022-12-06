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
    public class ApplyCouponHandler : IRequestHandler<ApplyCouponCommand, ResponseDto>
    {
        private readonly ICartRepository<CartHeader> _cartHeaderRepository;
        private readonly ILogger<ApplyCouponHandler> _logger;
        private readonly IMapper _mapper;
        protected ResponseDto responseDto;

        public ApplyCouponHandler(ICartRepository<CartHeader> cartHeaderRepository, ILogger<ApplyCouponHandler> logger, IMapper mapper)
        {
            _cartHeaderRepository = cartHeaderRepository;
            _logger = logger;
            _mapper = mapper;
            this.responseDto = new ResponseDto();
        }

        public async Task<ResponseDto> Handle(ApplyCouponCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var cartFromDb = _cartHeaderRepository.Query().FirstOrDefault(x => x.UserId == request.CartHeader.UserId);
                if (cartFromDb != null)
                {
                    cartFromDb.CouponCode = request.CartHeader.CouponCode;
                    var result = await _cartHeaderRepository.Update(cartFromDb);
                    if (result.Item2 is true) responseDto.Result = result.Item2;
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
