using AutoMapper;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.DTO;

namespace Mango.Services.ShoppingCartAPI.Models
{
    public class MappingConfig
    {
        //Mapping ProductDto to Product and Vice versa
        public static MapperConfiguration RegisterMaps()
        {
            var mappingconfig = new MapperConfiguration(config =>
            {
                config.CreateMap<CartHeaderDto, CartHeader>();
                config.CreateMap<CartHeader, CartHeaderDto>();
                //Another way to map vice versa
                config.CreateMap<CartDetails, CartDetailsDto>().ReverseMap();
            });
            return mappingconfig;
        }
    }
}
