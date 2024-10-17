using System;
using AutoMapper;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;
using System.Text.Json;
using Mango.Services.ShoppingCartAPI.Service.IService;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        private readonly ResponseDto _response;
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;
        private readonly IProductService _productService;
        private readonly ICouponService _couponService;
        public CartAPIController(AppDbContext db, IMapper mapper, IProductService productService,ICouponService couponService)
        {
            _db = db;
            _mapper = mapper;
            this._response = new ResponseDto();
            _productService = productService;
            _couponService = couponService;
        }
        [HttpGet("GetCart/{userId}")]
        public async Task<ResponseDto> GetCart(string userId)
        {
            try
            {
                CartDto cartDto = new()
                {
                    CartHeader=_mapper.Map<CartHeaderDto>(_db.CartHeaders.First(u=>u.UserId==userId)), 
                };

                cartDto.CartDetails = _mapper.Map<IEnumerable<CartDetailsDto>>(_db.CartDetails.Where(u =>
                u.CartHeaderId == cartDto.CartHeader.CartHeaderId));

                //Populate Total in Header which contains the total price
                var products = await _productService.GetAllProductAsync();
                foreach (var item in cartDto.CartDetails)
                {
                    var Id = item.ProductId;
                    if (Id <= cartDto.CartDetails.Count()-1)
                    {
                        item.Product = products.First(u => u.ProductId == Id);
                        cartDto.CartHeader.CartTotal += (item.Count * item.Product.Price);
                    }                    
                }

                //Apply coupon if any for the Discount
                if(!string.IsNullOrEmpty(cartDto.CartHeader.CouponCode))
                {
                    CouponDto coupon = await _couponService.GetCoupon(cartDto.CartHeader.CouponCode);
                    if(coupon!=null && cartDto.CartHeader.CartTotal>coupon.MinAmount)
                    {
                        cartDto.CartHeader.CartTotal -= coupon.DiscountAmount;
                        cartDto.CartHeader.Discount=coupon.DiscountAmount;
                    }
                }
                _response.Result = cartDto;


                return _response;
            }
            catch(Exception ex)
            {
                _response.IsSuccess= false;
                _response.Message= ex.Message.ToString();
            }
            return _response;
        }


        [HttpPost("ApplyCoupon")]
        public async Task<Object> ApplyCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                var cartFromDb =await _db.CartHeaders.FirstAsync(u => u.UserId == cartDto.CartHeader.UserId);
                var checkCouponExist =await _couponService.GetCoupon(cartDto.CartHeader.CouponCode);
                if(checkCouponExist!=null && !string.IsNullOrEmpty(checkCouponExist.CouponCode)) {
                    cartFromDb.CouponCode = cartDto.CartHeader.CouponCode;
                    _db.Update(cartFromDb);
                    await _db.SaveChangesAsync();
                    _response.Result = cartFromDb;
                }
                else
                {
                    _response.IsSuccess= false;
                    _response.Message = "Invalid coupon";
                }
                
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message.ToString();
            }
            return _response;
        }

        [HttpPost("RemoveCoupon")]
        public async Task<Object> RemoveCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                var cartFromDb = await _db.CartHeaders.FirstAsync(u => u.UserId == cartDto.CartHeader.UserId);
                cartFromDb.CouponCode =string.Empty;
                _db.Update(cartFromDb);
                await _db.SaveChangesAsync();
                _response.Result = "";
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message.ToString();
            }
            return _response;
        }

        [HttpPost("CartUpsert")]
        public async Task<ResponseDto> CartUpsert(CartDto cartDto)
        {
            try
            {
                //Retrieve only the CartHeader for the current logged User
                var cartHeaderFromDb =await _db.CartHeaders.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == cartDto.CartHeader.UserId);
                //Case1 if it's the 1st time the user add item to the cart
                if(cartHeaderFromDb == null)
                {
                    //create Header and Detaitls
                    CartHeader cartHeader= _mapper.Map<CartHeader>(cartDto.CartHeader);
                    _db.CartHeaders.Add(cartHeader);
                    await _db.SaveChangesAsync();
                    //Retrieve cartHeaderId
                    cartDto.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
                    _db.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                    await _db.SaveChangesAsync();
                }
                else
                {
                    //if header is not null
                    //check if details has same product
                    var cartDetailsFromDb= await _db.CartDetails.AsNoTracking().FirstOrDefaultAsync(u=>u.ProductId == cartDto.CartDetails.First().ProductId
                    && u.CartHeaderId==cartHeaderFromDb.CartHeaderId);
                    if(cartDetailsFromDb==null)
                    {
                        //Create an another CartDetails because it does not exist
                        cartDto.CartDetails.First().CartHeaderId = cartHeaderFromDb.CartHeaderId;
                        _db.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await _db.SaveChangesAsync();
                    }
                    else
                    {
                        //The product already does exist && Update Count in cartDetails
                        cartDto.CartDetails.First().Count += cartDetailsFromDb.Count;
                        cartDto.CartDetails.First().CartHeaderId = cartDetailsFromDb.CartHeaderId;
                        cartDto.CartDetails.First().CartDetailsId = cartDetailsFromDb.CartDetailsId;
                        _db.CartDetails.Update(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await _db.SaveChangesAsync();
                    }                    
                }
                _response.Result = cartDto;
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message= ex.Message.ToString();
            }
            return _response;
        }
        [HttpPost("RemoveCart")]
        public async Task<ResponseDto> RemoveCart([FromBody] int cartDetailsId)
        {
            try
            {
                CartDetails cartDetails= _db.CartDetails.First(u=>u.CartDetailsId==cartDetailsId);
                int totalCountOfCartItem=_db.CartDetails.Where(u=>u.CartHeaderId== cartDetails.CartHeaderId).Count();

                _db.CartDetails.Remove(cartDetails);
                if(totalCountOfCartItem==1)
                {
                    var cartHeaderToRemove = await _db.CartHeaders.FirstOrDefaultAsync(u => u.CartHeaderId == cartDetails.CartHeaderId);
                    _db.CartHeaders.Remove(cartHeaderToRemove);
                }
                await _db.SaveChangesAsync();
                _response.Result = true;
                return _response;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message=ex.Message.ToString();
            }
            return _response;
        }
    }
}
