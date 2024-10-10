using Mango.Services.ShoppingCartAPI.Models.DTO;
using Mango.Services.ShoppingCartAPI.Service.IService;
using Newtonsoft.Json;

namespace Mango.Services.ShoppingCartAPI.Service
{
    public class CouponService : ICouponService
    {
        IHttpClientFactory _httpClientFactory;
        public CouponService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<CouponDto> GetCoupon(string code)
        {
            var client = _httpClientFactory.CreateClient("Coupon");
            var response= await client.GetAsync($"api/coupon/GetByCode/{code}");
            var apiContent= await response.Content.ReadAsStringAsync();
            var resp= JsonConvert.DeserializeObject<ResponseDto>(apiContent);
            if(resp != null)
            {
                return JsonConvert.DeserializeObject<CouponDto>(resp.Result.ToString());
            }
            return new CouponDto();
        }
    }
}
