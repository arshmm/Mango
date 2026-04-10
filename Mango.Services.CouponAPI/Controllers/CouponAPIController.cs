using AutoMapper;
using Mango.Services.CouponAPI.Data;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.CouponAPI.Controllers
{
    [Route("api/coupon")]
    [ApiController]
    [Authorize]
    public class CouponAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private ResponseDto _response;
        private IMapper _mapper;
        public CouponAPIController(AppDbContext db, IMapper mapper)

        {
            _db = db;
            _response = new ResponseDto();
            _mapper = mapper;
        }
        [HttpGet]
        public ResponseDto Get()
        {
            try
            {
                IEnumerable<Coupon> objList = _db.Coupons.ToList();
                _response.Result = _mapper.Map<IEnumerable<CouponDto>>(objList);

            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }
            return _response;
        }

        [HttpGet]
        [Route("{id:int}")]
        public ResponseDto Get(int id)
        {
            try
            {
                Coupon obj = _db.Coupons.First(u => u.CouponId == id);
                _response.Result = _mapper.Map<CouponDto>(obj);

            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }
            return _response;
        }

        [HttpGet]
        [Route("/GetByCode/{code}")]
        public ResponseDto GetByCode(string code)
        {
            try
            {
                Coupon obj = _db.Coupons.First(u => u.CouponCode.ToLower() == code);
                _response.Result = _mapper.Map<CouponDto>(obj);

            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }
            return _response;
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Post([FromBody] CouponDto couponDto)
        {
            try
            {
                _db.Coupons.Add(_mapper.Map<Coupon>(couponDto));
                _db.SaveChanges();
                _response.Result = couponDto;

            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }
            return _response;
        }

        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Put([FromBody] CouponDto couponDto)
        {
            try
            {
                _db.Coupons.Update(_mapper.Map<Coupon>(couponDto));
                _db.SaveChanges();
                _response.Result = couponDto;

            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }
            return _response;
        }


        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Delete(int id)
        {
            try
            {
                _db.Coupons.Remove(_db.Coupons.First(u => u.CouponId == id));
                _db.SaveChanges();


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
