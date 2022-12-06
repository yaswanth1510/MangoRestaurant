using AutoMapper;
using Mango.Services.ProductAPI.Domain.Enitities;
using Mango.Services.ProductAPI.Domain.Models.DTO;

namespace Mango.Services.ProductAPI.Domain
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<Product, ProductDto>();
                config.CreateMap<ProductDto, Product>();
            });

            return mappingConfig;
        }
    }
}
