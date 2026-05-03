using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }
        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartBasedOnLoggedinUser());
        }

        public async Task<IActionResult> Remove(int cartDetailsId)
        {
            //var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDto? res = await _cartService.RemoveFromCartAsync(cartDetailsId);
            if (res != null && res.IsSuccess)
            {

                TempData["success"] = "Cart updated successfully";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();

        }
        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
        {

            cartDto.CartDetails = [];

            ResponseDto? res = await _cartService.ApplyCouponAsync(cartDto);
            if (res != null && res.IsSuccess)
            {

                TempData["success"] = "Cart updated successfully";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
        {

            cartDto.CartDetails = [];
            cartDto.CartHeader.CouponCode = "";

            ResponseDto? res = await _cartService.ApplyCouponAsync(cartDto);
            if (res != null && res.IsSuccess)
            {

                TempData["success"] = "Cart updated successfully";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();

        }

        private async Task<CartDto> LoadCartBasedOnLoggedinUser()
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDto? res = await _cartService.GetCartByUserIdAsync(userId);
            if (res != null && res.IsSuccess)
            {
                CartDto cartDto = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(res.Result));
                return cartDto;
            }
            return new CartDto();

        }
    }

}
