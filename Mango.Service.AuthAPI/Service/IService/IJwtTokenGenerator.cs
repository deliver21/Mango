using Mango.Service.AuthAPI.Models;

namespace Mango.Service.AuthAPI.Service.IService
{
    public interface IJwtTokenGenerator
    {
        //Passing the role through IEnumerable<string> roles , user can have multiple roles
        string GenerateToken(ApplicationUser applicationUser , IEnumerable<string> roles);
    }
}
