using Mango.Services.ProductAPI.Domain.Commands;
using Mango.Services.ProductAPI.Domain.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ProductAPI.Controllers
{
    public class ProductAPIController : APIControllerBase
    {

        [HttpGet]
        public async Task<object> Get()
        {
            return await Mediator.Send(new GetAllProductsQuery());
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<object> Get(int id)
        {
            return await Mediator.Send(new GetProductByIdQuery(id));
        }

        [HttpPost]
        [Authorize]
        public async Task<object> Post([FromBody] CreateUpdateProductCommand productDto)
        {
            return await Mediator.Send(productDto);
        }

        [HttpPut]
        [Authorize]
        public async Task<object> Put([FromBody] CreateUpdateProductCommand productDto)
        {
            return await Mediator.Send(productDto);
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        [Route("{id}")]
        public async Task<object> Delete(int id)
        {
            return await Mediator.Send(new DeleteProductCommand(id));
        }
    }
}
