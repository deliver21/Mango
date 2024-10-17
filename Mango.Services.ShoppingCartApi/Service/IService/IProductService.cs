using Mango.Services.ShoppingCartAPI.Models.DTO;

namespace Mango.Services.ShoppingCartAPI
{
    public interface IProductService
    {
        Task <List<ProductDto>?> GetAllProductAsync();
        //Task<ResponseDto?> GetProductByIdAsync(int id);        
    }
}
