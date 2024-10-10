using AutoMapper;
using Mango.Services.CouponAPI.Models.DTO;

namespace Mango.Services.CouponAPI.Models
{
    public class MappingConfig
    {
        //Mapping CouponDto to Coupon and Vice versa
        public static MapperConfiguration RegisterMaps()
        {
            var mappingconfig = new MapperConfiguration(config =>
            {
                config.CreateMap<CouponDto, Coupon>();
                config.CreateMap<Coupon, CouponDto>();
            });
            return mappingconfig;
        }
    }
}
