using Mango.Web.Service.IService;
using Mango.Web.Utilities;
using Newtonsoft.Json.Linq;

namespace Mango.Web.Service
{
    public class TokenProvider : ITokenProvider
    { 
        //A must to work with Token  ,** cookie
        private readonly IHttpContextAccessor _contextAccessor;
        public TokenProvider(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
        public void ClearToken()
        {
            _contextAccessor.HttpContext?.Response.Cookies.Delete(SD.tokenCookie);
        }

        public string? GetToken()
        {
            string? token = null;
            bool? hastoken = _contextAccessor.HttpContext?.Request.Cookies.TryGetValue(SD.tokenCookie,out token); 
            return hastoken is true ? token:null;
        }

        public void SetToken(string? token)
        {
            _contextAccessor.HttpContext?.Response.Cookies.Append(SD.tokenCookie,token);
        }
    }
}
