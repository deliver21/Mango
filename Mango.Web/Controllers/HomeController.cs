using Mango.Web.Models;
using Mango.Web.Service;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICartService _cartService;

        public HomeController(IProductService productService, ICartService cartService)
        {
            _productService = productService;
            _cartService = cartService;
        }

        public async Task <IActionResult> Index()
        {
            ResponseDto? responseDto= new ResponseDto();
            responseDto = await _productService.GetAllProductAsync();
            List<ProductDto?> list = new List<ProductDto>();
            if (responseDto != null && responseDto.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<ProductDto>>(responseDto.Result.ToString());
                return View(list);
            }
            return View(list);
        }

        public async Task <IActionResult> ProductDetails(int productId)
        {
            ResponseDto responseDto = new();
            responseDto=await _productService.GetProductByIdAsync(productId);
            if(responseDto!=null && responseDto.IsSuccess)
            {
                ProductDto productDto = new();
                productDto = JsonConvert.DeserializeObject<ProductDto>(responseDto.Result.ToString());
                return View(productDto);
            }
            return NotFound();
        } 

        [Authorize]
        [HttpPost]
        [ActionName("ProductDetails")]
        public async Task<IActionResult> ProductDetails(ProductDto productDto)
        {
            CartDto cartDto = new CartDto()
            {
                CartHeader = new CartHeaderDto()
                {
                    UserId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value,
                },
            };
            //Populate cardDetailsDto
            CartDetailsDto cartDetailsDto = new CartDetailsDto()
            {
                Count = productDto.Count,
                ProductId = productDto.ProductId
            };
            List<CartDetailsDto> cartDetailsDtos = new List<CartDetailsDto>()
            { 
                cartDetailsDto
            };
            cartDto.CartDetails = cartDetailsDtos;
            var responseDto = await _cartService.CartUpsertAsync(cartDto);

            if (responseDto != null && responseDto.IsSuccess)
            {
                TempData["success"] = $"The product {productDto.Name} is successfully added to the Cart";
                return RedirectToAction("Index");
            }
            return View(productDto);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}