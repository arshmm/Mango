using AutoMapper;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private ResponseDto _response;
        private IMapper _mapper;
        private IProductService _productService;
        private ICouponService _couponService;
        public CartAPIController(AppDbContext db, IMapper mapper, IProductService productService, ICouponService couponService)

        {
            _db = db;
            _response = new ResponseDto();
            _mapper = mapper;
            _productService = productService;
            _couponService = couponService;
        }

        [HttpGet("GetCart/{userid}")]
        public async Task<ResponseDto> GetCart(string userid)
        {
            try
            {
                CartDto cart = new()
                {
                    CartHeader = _mapper.Map<CartHeaderDto>(_db.CartHeaders.First(u => u.UserId == userid))
                };
                cart.CartDetails = _mapper.Map<IEnumerable<CartDetailsDto>>
                    (_db.CartDetails.Where(u => u.CartHeaderId == cart.CartHeader.CartHeaderId));

                foreach (var item in cart.CartDetails)
                {
                    item.Product = await _productService.GetProductbyId(item.ProductId);

                    cart.CartHeader.CartTotal += (item.Count * item.Product.Price);
                }
                if (!string.IsNullOrEmpty(cart.CartHeader.CouponCode))
                {
                    CouponDto couponDto = await _couponService.GetCouponByCode(cart.CartHeader.CouponCode);
                    if (couponDto != null && cart.CartHeader.CartTotal > couponDto.MinAmount)
                    {
                        cart.CartHeader.CartTotal -= couponDto.DiscountAmount;
                        cart.CartHeader.Discount = couponDto.DiscountAmount;
                    }
                }


                _response.Result = cart;

            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;

            }
            return _response;

        }

        [HttpPost("CartUpsert")]
        public async Task<ResponseDto> Upsert(CartDto cartDto)
        {
            try
            {
                var cartHeaderFromDb = await _db.CartHeaders.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == cartDto.CartHeader.UserId);
                if (cartHeaderFromDb == null)
                {
                    //create header and details
                    CartHeader cartHeader = _mapper.Map<CartHeader>(cartDto.CartHeader);
                    _db.CartHeaders.Add(cartHeader);
                    await _db.SaveChangesAsync();
                    cartDto.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
                    CartDetails cartDetails = _mapper.Map<CartDetails>(cartDto.CartDetails.First());
                    _db.CartDetails.Add(cartDetails);
                    await _db.SaveChangesAsync();
                }
                else
                {
                    var cartDetailsFromDb = await _db.CartDetails.AsNoTracking().FirstOrDefaultAsync(
                        u => u.ProductId == cartDto.CartDetails.First().ProductId
                        && u.CartHeaderId == cartHeaderFromDb.CartHeaderId);
                    if (cartDetailsFromDb == null)
                    {
                        //create card details
                        cartDto.CartDetails.First().CartHeaderId = cartHeaderFromDb.CartHeaderId;
                        CartDetails cartDetails = _mapper.Map<CartDetails>(cartDto.CartDetails.First());
                        _db.CartDetails.Add(cartDetails);
                        await _db.SaveChangesAsync();

                    }
                    else
                    {
                        cartDto.CartDetails.First().Count += cartDetailsFromDb.Count;
                        cartDto.CartDetails.First().CartHeaderId = cartDetailsFromDb.CartHeaderId;
                        cartDto.CartDetails.First().CartDetailsId = cartDetailsFromDb.CartDetailsId;
                        CartDetails cartDetails = _mapper.Map<CartDetails>(cartDto.CartDetails.First());
                        _db.CartDetails.Update(cartDetails);
                        await _db.SaveChangesAsync();

                    }
                }
                _response.Result = cartDto;
            }
            catch (Exception ex)
            {

                _response.Message = ex.Message.ToString();
                _response.IsSuccess = false;
            }
            return _response;
        }
        [HttpPost("RemoveCart")]
        public async Task<ResponseDto> Remove([FromBody] int cartDetailsId)
        {
            try
            {
                CartDetails cartDetails = _db.CartDetails.First(u => u.CartDetailsId == cartDetailsId);
                int totalCountOfDetails = _db.CartDetails.Where(u => u.CartHeaderId == cartDetails.CartHeaderId).Count();
                _db.CartDetails.Remove(cartDetails);
                if (totalCountOfDetails == 1)
                {
                    var cartHeaderToRemove = await _db.CartHeaders
                     .FirstOrDefaultAsync(u => u.CartHeaderId == cartDetails.CartHeaderId);

                    _db.CartHeaders.Remove(cartHeaderToRemove);
                    // var res = await _db.CartHeaders.Where(u => u.CartHeaderId == cartDetails.CartHeaderId).ExecuteDeleteAsync();
                    //the above line donest work , have to understand why , something to do with concurrency
                }
                await _db.SaveChangesAsync();

                _response.IsSuccess = true;
            }
            catch (Exception ex)
            {

                _response.Message = ex.Message.ToString();
                _response.IsSuccess = false;
            }
            return _response;
        }

        [HttpPost("ApplyCoupon")]
        public async Task<ResponseDto> ApplyCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                var cartFromDb = await _db.CartHeaders.FirstAsync(u => u.UserId == cartDto.CartHeader.UserId);
                cartFromDb.CouponCode = cartDto.CartHeader.CouponCode;
                _db.CartHeaders.Update(cartFromDb);
                await _db.SaveChangesAsync();
                _response.IsSuccess = true;
            }
            catch (Exception ex)
            {

                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }
            return _response;
        }

        [HttpPost("RemoveCoupon")]
        public async Task<ResponseDto> RemoveCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                var cartFromDb = await _db.CartHeaders.FirstAsync(u => u.UserId == cartDto.CartHeader.UserId);
                cartFromDb.CouponCode = "";
                _db.CartHeaders.Update(cartFromDb);
                await _db.SaveChangesAsync();
                _response.IsSuccess = true;
            }
            catch (Exception ex)
            {

                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }
            return _response;
        }

    }
}
