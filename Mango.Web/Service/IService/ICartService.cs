using Mango.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Web.Service.IService
{
    public interface ICartService
    {
        Task<ResponseDto ?> GetCartByUserIdAsync(string userId);
        Task<ResponseDto?> ApplyCouponAsync(CartDto cartDto);
        Task<ResponseDto?> RemoveCouponAsync(CartDto cartDto);
        Task<ResponseDto> CartUpsertAsync(CartDto cartDto);
        Task<ResponseDto> RemoveCartAsync(int cartDetailsId);
        //Task<ResponseDto> CreateCouponAsync(CouponDto couponDto);
    }
}
