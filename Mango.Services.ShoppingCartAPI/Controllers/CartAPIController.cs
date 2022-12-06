using Mango.MessageBus;
using Mango.Services.ShoppingCartAPI.Domain.Commands;
using Mango.Services.ShoppingCartAPI.Domain.Messages;
using Mango.Services.ShoppingCartAPI.Domain.Models.Dto;
using Mango.Services.ShoppingCartAPI.Domain.Notifications;
using Mango.Services.ShoppingCartAPI.Domain.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    public class CartAPIController : ApiControllerBase
    {
        private readonly IPublisher _publisher;

        public CartAPIController(IPublisher publisher)
        {
            _publisher = publisher;
        }

        [HttpGet("GetCart/{userId}")]
        public async Task<object> GetCart(string userId)
        {
            return await Mediator.Send(new GetCartByIdQuery(userId));
        }


        [HttpPost("AddCart")]
        public async Task<object> AddCart(CreateUpdateCartCommand cartDto)
        {
            return await Mediator.Send(cartDto);
        }

        [HttpPost("UpdateCart")]
        public async Task<object> UpdateCart(CreateUpdateCartCommand cartDto)
        {
            return await Mediator.Send(cartDto);
        }

        [HttpPost("RemoveCart")]
        public async Task<object> RemoveCart([FromBody] int cartDetailsId)
        {
            return await Mediator.Send(new RemoveCartCommand(cartDetailsId));
        }

        [HttpPost("ApplyCoupon")]
        public async Task<object> ApplyCoupon([FromBody] ApplyCouponCommand cartDto)
        {
            return await Mediator.Send(cartDto);
        }

        [HttpPost("RemoveCoupon")]
        public async Task<object> RemoveCoupon([FromBody] string UserId)
        {
            return await Mediator.Send(new RemoveCouponCommand(UserId));
        }

        [HttpPost("Checkout")]
        public async Task<object> Checkout(CheckoutHeaderDto checkoutHeader)
        {
            await _publisher.Publish(new CheckoutNotification(checkoutHeader));
            return await Mediator.Send(new ClearCartCommand(checkoutHeader.UserId));
        }
    }
}
