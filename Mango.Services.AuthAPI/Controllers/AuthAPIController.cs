using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService _authService;
        protected ResponseDto _response;

        public AuthAPIController(IAuthService authService)
        {
            _authService = authService;
            _response = new();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto model)
        {
            var err = await _authService.Register(model);
            if (!string.IsNullOrEmpty(err))
            {
                _response.IsSuccess = false;
                _response.Message = err;
                return BadRequest(_response);
            }
            return Ok(_response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            var res = await _authService.Login(model);
            if (res.User == null)
            {
                _response.IsSuccess = false;
                _response.Message = "Username or password incorrect";
                return BadRequest(_response);
            }
            _response.Result = res;

            return Ok(_response);
        }

        [HttpPost("assignrole")]
        public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDto model)
        {
            var res = await _authService.AssignRole(model.Email, model.RoleName.ToUpper());
            if (!res)
            {
                _response.IsSuccess = false;
                _response.Message = "Error occured";
                return BadRequest(_response);
            }
            return Ok(_response);
        }


    }

}
