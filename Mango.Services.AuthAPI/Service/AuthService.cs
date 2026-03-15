using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Identity;

namespace Mango.Services.AuthAPI.Service
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthService(AppDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDto.UserName.ToLower());
            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);
            if (user == null || isValid == false)
            {

                return new LoginResponseDto { User = null, token = string.Empty };

            }
            // genrate jwt

            UserDto userDto = new()
            {
                Email = user.Email,
                ID = user.Id,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber
            };

            return new LoginResponseDto { User = userDto, token = string.Empty };
        }

        public async Task<string> Register(RegistrationRequestDto registrationRequestDto)
        {
            ApplicationUser user = new()
            {
                UserName = registrationRequestDto.Email,
                Email = registrationRequestDto.Email,
                NormalizedEmail = registrationRequestDto.Email.ToUpper(),
                Name = registrationRequestDto.Name,
                PhoneNumber = registrationRequestDto.PhoneNumber
            };
            try
            {
                var res = await _userManager.CreateAsync(user, registrationRequestDto.Password);
                if (res.Succeeded)
                {
                    var userTOreturn = _db.ApplicationUsers.First(u => u.UserName == user.UserName);
                    UserDto userDto = new()
                    {
                        Email = userTOreturn.Email,
                        ID = userTOreturn.Id,
                        Name = userTOreturn.Name,
                        PhoneNumber = userTOreturn.PhoneNumber
                    };
                    return "";

                }
                else { return res.Errors.FirstOrDefault().Description; }
            }
            catch (Exception ex)
            {

            }
            return "Error Occured";

        }
    }
}
