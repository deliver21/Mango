using System;
using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    [Authorize]
    public class CouponController : Controller
    {
        private readonly ICouponService _couponService;
        public CouponController(ICouponService couponService)
        { 
            _couponService = couponService;
        }
        public async Task<IActionResult> CouponIndex()
        {
            List<CouponDto> list = new();
            ResponseDto? response = await _couponService.GetAllCouponAsync();
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<CouponDto>>(response.Result.ToString());
            }
            return View(list);
        }
        [Authorize(Roles = "ADMIN")]
        public IActionResult CouponCreate()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> CouponCreate(CouponDto coupon)
        {
            if(ModelState.IsValid)
            {
                ResponseDto? response= await _couponService.CreateCouponAsync(coupon);
                if(response!=null && response.IsSuccess)
                {
                    TempData["success"] = "Coupon created successfully";
                    return RedirectToAction("CouponIndex");
                }
            }
            return View();
            
        }
        public async Task<IActionResult> CouponDelete(int couponId)
        {
            CouponDto coupon = new();
            ResponseDto? response = await _couponService.GetCouponByIdAsync(couponId);
            if (response != null && response.IsSuccess)
            {
                coupon = JsonConvert.DeserializeObject<CouponDto>(response.Result.ToString());
                return View(coupon);
            }
            return NotFound();
            
        }
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> CouponDelete(CouponDto coupon)
        {
            if (ModelState.IsValid)
            {
                ResponseDto? response = await _couponService.DeleteCouponAsync(coupon.CouponId);
                if (response != null && response.IsSuccess)
                {
                    TempData["succcess"] = "Coupon deleted successfully";
                    return RedirectToAction("CouponIndex");
                }
            }
            else
            {
                TempData["error"] = "Couldn't get to the point";
            }
            return View(coupon);
        }
    }
}
