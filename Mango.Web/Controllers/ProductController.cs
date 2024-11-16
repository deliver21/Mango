using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Mango.Web.Models;
using Mango.Web.Service;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace Mango.Web.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService) 
        {
            _productService = productService;
        }        
        public async Task<IActionResult> ProductIndex()
        {
            List<ProductDto> list = new();
            ResponseDto? response = await _productService.GetAllProductAsync();
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<ProductDto>>(response.Result.ToString());
            }
            return View(list);
        }
        public IActionResult ProductCreate()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> ProductCreate(ProductDto product)
        {
            if (ModelState.IsValid)
            {
                ResponseDto? response = await _productService.CreateProductAsync(product);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Product created successfully";
                    return RedirectToAction("ProductIndex");
                }
            }
            return View();
        }
        public async Task<IActionResult> ProductDelete(int productId)
        {
            ProductDto product = new();
            ResponseDto? response = await _productService.GetProductByIdAsync(productId);
            if (response != null && response.IsSuccess)
            {                
                product = JsonConvert.DeserializeObject<ProductDto>(response.Result.ToString());
                return View(product);
            }
            return NotFound();
        }
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> ProductDelete(ProductDto product)
        {
                ResponseDto? response = await _productService.DeleteProductAsync(product.ProductId);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Product deleted successfully";
                    return RedirectToAction("ProductIndex");
                }
                else
                {
                    TempData["error"] = "The App couldn't delete the product";
                }            
            return View(new ProductDto{ProductId = product.ProductId , Name=product.Name,Description=product.Description ,
                CategoryName = product.CategoryName ,Price = product.Price , ImageUrl = product.ImageUrl
            });
        }
        public async Task <IActionResult> ProductEdit(int productId)
        {
            ResponseDto response = new();
            response = await _productService.GetProductByIdAsync(productId);
            ProductDto product = new();
            if (response != null && response.IsSuccess)
            {
                product = JsonConvert.DeserializeObject<ProductDto>(response.Result.ToString());
                return View(product);
            }
            return RedirectToAction(nameof(ProductIndex));
        }
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task <IActionResult> ProductEdit(ProductDto product)
        {
            ResponseDto? response = await _productService.UpdateProductAsync(product);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Product deleted successfully";
                return RedirectToAction("ProductIndex");
            }
            else
            {
                TempData["error"] = "The App couldn't delete the product";
            }
            return View(new ProductDto
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                CategoryName = product.CategoryName,
                Price = product.Price,
                ImageUrl = product.ImageUrl
            });
        }
    }
}