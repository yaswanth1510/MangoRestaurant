using AutoMapper;
using Mango.Services.OrderAPI.Messages;
using Mango.Services.OrderAPI.Models;

namespace Mango.Services.OrderAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<OrderHeader, CheckoutHeaderDto>().ReverseMap();
                config.CreateMap<OrderDetails, CartDetailsDto>().ReverseMap();
            });

            return mappingConfig;
        }
    }
}
