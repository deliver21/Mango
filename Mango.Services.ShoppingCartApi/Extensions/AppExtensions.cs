using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Mango.Services.ShoppingCartAPI.Extensions
{
    public static class  WebApplicationBuilderExtensions
    {
        //WebApplicationBuilder is the type of builder in Program.cs
        public static WebApplicationBuilder AddAppAuthentification(this WebApplicationBuilder builder)
        {
            //Get the token Section in appJson
            var sectionSettings = builder.Configuration.GetSection("ApiSettings");

            //Validate Token
            var secret = sectionSettings.GetValue<string>("Secret");
            var issuer = sectionSettings.GetValue<string>("Issuer");
            var audience = sectionSettings.GetValue<string>("Audience");

            var key = Encoding.ASCII.GetBytes(secret);
            //End

            //Add Authentification Service
            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    ValidateAudience = true
                };
            });
            //End
            builder.Services.AddAuthorization();
            return builder;

        }
    }
}
