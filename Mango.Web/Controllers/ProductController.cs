using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        public async Task<IActionResult> ProductIndex()
        {
            List<ProductDto>? list = new();

            ResponseDto response = await _productService.GetAllProductsAsync();

            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response.Message;
            }

            return View(list);
        }

        public async Task<IActionResult> ProductCreate()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ProductCreate(ProductDto model)
        {
            if (ModelState.IsValid)
            {
                ResponseDto response = await _productService.CreateProductAsync(model);

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Product created successfully";
                    return RedirectToAction(nameof(ProductIndex));
                }
                else
                {
                    TempData["error"] = response.Message;
                }

            }
            return View(model);
        }

        public async Task<IActionResult> ProductDelete(int id)

        {
            ProductDto product = new();
            ResponseDto res = await _productService.GetProductByIdAsync(id);

            if (res != null && res.IsSuccess)
            {
                product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(res.Result));
                return View(product);
            }
            else
            {
                TempData["error"] = res.Message;
            }


            return NotFound();
        }


        [HttpPost]
        public async Task<IActionResult> ProductDelete(ProductDto product)

        {

            ResponseDto res = await _productService.DeleteProductAsync(product.ProductId);

            if (res != null && res.IsSuccess)
            {
                TempData["success"] = "Product deleted successfully";
                return RedirectToAction(nameof(ProductIndex));
            }
            else
            {
                TempData["error"] = res.Message;
            }



            return View(product);
        }


        public async Task<IActionResult> ProductEdit(int id)

        {
            ProductDto product = new();
            ResponseDto res = await _productService.GetProductByIdAsync(id);

            if (res != null && res.IsSuccess)
            {
                product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(res.Result));
                return View(product);
            }
            else
            {
                TempData["error"] = res.Message;
            }


            return NotFound();
        }


        [HttpPost]
        public async Task<IActionResult> ProductEdit(ProductDto product)

        {

            ResponseDto res = await _productService.UpdateProductAsync(product);

            if (res != null && res.IsSuccess)
            {
                TempData["success"] = "Product Updated successfully";
                return RedirectToAction(nameof(ProductIndex));
            }
            else
            {
                TempData["error"] = res.Message;
            }



            return View(product);
        }

    }
}
