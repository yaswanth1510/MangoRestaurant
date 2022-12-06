using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Mango.MessageBus;
using Mango.Services.ShoppingCartAPI.Domain.Entities;
using Mango.Services.ShoppingCartAPI.Domain.Interface;
using Mango.Services.ShoppingCartAPI.Domain.Models.Dto;
using Mango.Services.ShoppingCartAPI.Domain.Notifications;
using Mango.Services.ShoppingCartAPI.Domain.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Mango.Services.ShoppingCartAPI.Domain.Handlers
{
    public class MessageHandler : INotificationHandler<CheckoutNotification>
    {
        private readonly ILogger<MessageHandler> _logger;
        protected ResponseDto responseDto;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IMessageBus _messageBus;

        public MessageHandler(ILogger<MessageHandler> logger, IMediator mediator, IMapper mapper, IMessageBus messageBus)
        {
            _logger = logger;
            responseDto = new ResponseDto();
            _mediator = mediator;
            _mapper = mapper;
            _messageBus = messageBus;
        }

        public async Task Handle(CheckoutNotification notification, CancellationToken cancellationToken)
        {
            try
            {
                responseDto = await _mediator.Send(new GetCartByIdQuery(notification.CheckoutHeaderDto.UserId));
                CartDto cart = _mapper.Map<CartDto>(responseDto.Result);
                notification.CheckoutHeaderDto.CartDetails = cart.CartDetails;
                await _messageBus.PublishMessage(notification.CheckoutHeaderDto, "checkoutmessagetopic");
            }
            catch (Exception e)
            {
                responseDto.IsSuccess = false;
                responseDto.ErrorMessage = new List<string> { e.Message.ToString() };
                _logger.LogError(e.Message.ToString());
            }
        }
    }
}
