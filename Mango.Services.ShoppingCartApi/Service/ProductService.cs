using Mango.Services.ShoppingCartAPI.Utilities;
using Mango.Services.ShoppingCartAPI.IService;
using Mango.Services.ShoppingCartAPI.Models.DTO;
using Newtonsoft.Json;

namespace Mango.Services.ShoppingCartAPI
{
    public class ProductService : IProductService
    {
        IHttpClientFactory _httpClientFactory;
        public ProductService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<ProductDto?>> GetAllProductAsync()
        {
           var client = _httpClientFactory.CreateClient("Product");
           var response = await client.GetAsync($"api/product");
           var apiContent = await response.Content.ReadAsStringAsync();
           var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
           if (resp != null && resp.Result != null)
           {
             return JsonConvert.DeserializeObject<List<ProductDto>>(resp.Result.ToString());
           }
            return new List<ProductDto?>();
        }
    }
}
