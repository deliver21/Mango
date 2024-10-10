using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Utilities;
using Microsoft.AspNetCore.Http.Connections;
using System.Text;
using Mango.Services.ShoppingCartAPI.IService;
using Mango.Services.ShoppingCartAPI.Models.DTO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mango.Services.ShoppingCartAPI
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        //private readonly ITokenProvider _tokenProvider;
        public BaseService(IHttpClientFactory httpClientFactory/*ITokenProvider tokenProvider*/)
        {
            _httpClientFactory = httpClientFactory;
            //_tokenProvider = tokenProvider;
        }
        public async Task<ResponseDto?> SendAsync(RequestDto requestDto, bool withBearer=true)
        {
            try
            {
                HttpClient client = _httpClientFactory.CreateClient("MangoAPI");
                HttpRequestMessage message = new HttpRequestMessage();
                message.Headers.Add("Accept", "application/json");
                //token from the Web project to the CouponApi project
                //if(withBearer)
                //{
                //    var token = _tokenProvider.GetToken();
                //    message.Headers.Add("Authorization",$"Bearer {token}");
                //}
                message.RequestUri = new Uri(requestDto.Url);
                if (requestDto.Data != null)
                {
                    message.Content = new StringContent(JsonSerializer.Serialize(requestDto.Data), Encoding.UTF8, "application/json");
                }
                HttpResponseMessage? apiResponse = null;
                switch (requestDto.ApiType)
                {
                    case SD.ApiType.POST:
                        message.Method = HttpMethod.Post;
                        break;
                    case SD.ApiType.PUT:
                        message.Method = HttpMethod.Put;
                        break;
                    case SD.ApiType.DELETE:
                        message.Method = HttpMethod.Delete;
                        break;
                    default:
                        message.Method = HttpMethod.Get;
                        break;
                }
                apiResponse = await client.SendAsync(message);
                switch (apiResponse.StatusCode)
                {
                    case System.Net.HttpStatusCode.NotFound:
                        return new() { IsSuccess = false, Message = "Not Found" };
                    case System.Net.HttpStatusCode.Forbidden:
                        return new() { IsSuccess = false, Message = "Access Denied" };
                    case System.Net.HttpStatusCode.Unauthorized:
                        return new() { IsSuccess = false, Message = "Unauthorised" };
                    case System.Net.HttpStatusCode.InternalServerError:
                        return new() { IsSuccess = false, Message = "Internal Server Error" };
                    default:
                        var apiContent = await apiResponse.Content.ReadAsStringAsync();
                        var apiResponseDto = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseDto>(apiContent);                        
                        return apiResponseDto;
                }
            }
            catch(Exception ex)
            {
                var dto = new ResponseDto()
                {
                    Message = ex.Message.ToString(),
                    IsSuccess = false
                };
              return dto;                 
            }            
        }
    }
}
