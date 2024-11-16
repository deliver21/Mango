using Mango.Services.EmailAPI.Models.DTO;

namespace Mango.Services.EmailAPI.Services.IServices
{
    public interface IEmailService
    {
        Task EmailCartAndLog(CartDto cartDto);
    }
}
