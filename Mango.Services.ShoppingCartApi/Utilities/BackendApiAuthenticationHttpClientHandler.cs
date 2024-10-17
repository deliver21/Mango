using Microsoft.AspNetCore.Authentication;

namespace Mango.Services.ShoppingCartAPI.Utilities
{
    //DelegatingHandler sort of Middleware but only on theclient side
    public class BackendApiAuthenticationHttpClientHandler:DelegatingHandler
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public BackendApiAuthenticationHttpClientHandler(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,CancellationToken cancellation)
        {
            var token = await _contextAccessor.HttpContext.GetTokenAsync("access_token");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",token);
            return await base.SendAsync(request, cancellation);
        }
    }
}
