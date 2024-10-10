using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utilities;
using System;

namespace Mango.Web.Service
{
    public class ProductService : IProductService
    {
        private readonly IBaseService _baseService;
        public ProductService(IBaseService baseService) 
        { 
            _baseService = baseService;
        }
        public async Task<ResponseDto?> CreateProductAsync(ProductDto productDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType=SD.ApiType.POST,
                Data=productDto,
                Url=SD.ProductAPIBase + "/api/product"
            });

        }

        public async Task<ResponseDto?> DeleteProductAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.DELETE,
                Data=id,
                Url = SD.ProductAPIBase + "/api/product/"+id
            });
        }

        public async Task<ResponseDto?> GetAllProductAsync()
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.ProductAPIBase + "/api/product"
            });
        }

        public async Task<ResponseDto?> GetProductByIdAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Data=id,
                Url = SD.ProductAPIBase + "/api/product/"+ id
            });
        }

        public async Task<ResponseDto?> UpdateProductAsync(ProductDto couponDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.PUT,
                Data = couponDto,
                Url = SD.ProductAPIBase + "/api/product"
            });
        }
    }
}
