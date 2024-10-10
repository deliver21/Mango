using static Mango.Services.ShoppingCartAPI.Utilities.SD;

namespace Mango.Services.ShoppingCartAPI.Models.DTO
{
    public class RequestDto
    {
        public ApiType ApiType { get; set; } = ApiType.GET;
        public string Url { get; set; }
        public object Data { get; set; }    
        public string AccessToken { get; set; }
    }
}
