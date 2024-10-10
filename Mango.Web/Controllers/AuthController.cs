using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utilities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Security.AccessControl;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ITokenProvider _tokenProvider;
        public AuthController(IAuthService authService, ITokenProvider tokenProvider)
        {
            _authService = authService;
            _tokenProvider = tokenProvider;
        }
        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDto loginRequestDto= new LoginRequestDto(); 
            return View(loginRequestDto);
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto?model)
        {
            if (ModelState.IsValid)
            {
                var responseDto = await _authService.LoginAsync(model);
                if (responseDto != null && responseDto.IsSuccess == true)
                {
                    //Conversion of responseDto.Result
                    
                    var loginResponseDto = JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(responseDto.Result));

                    //Assign User
                    await SignInUser(loginResponseDto);

                    //Get the Token Provider
                    _tokenProvider.SetToken(loginResponseDto.Token);
                    return RedirectToAction("Index","Home");
                }
                else
                {
                    TempData["error"] = responseDto.Message;
                }
            }
            return View(model);
        }
        public IActionResult Register()
        {
            var roleList=new List<SelectListItem>()
            { new SelectListItem{Text=SD.RoleAdmin,Value = SD.RoleAdmin},
              new SelectListItem{Text=SD.RoleCustomer,Value = SD.RoleCustomer}
            };
            ViewBag.RoleList=roleList;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDto registrationRequestDto)
        {
            if(ModelState.IsValid)
            {
                var response = await _authService.RegisterAsync(registrationRequestDto);
                ResponseDto assignRole;
                if (response!=null && response.IsSuccess==true)
                {
                    if(string.IsNullOrEmpty(registrationRequestDto.Role))
                    {
                        registrationRequestDto.Role = SD.RoleCustomer;
                    }
                    assignRole = await _authService.AssingRoleAsync(registrationRequestDto);
                    if(assignRole!=null && assignRole.IsSuccess) {
                        TempData["success"] = "Registration is successful";
                        return RedirectToAction("Login");
                    }
                }
                //Also scope user who wanna register twice
                else
                {
                    TempData["error"] = response.Message;
                }
            }
            var roleList = new List<SelectListItem>()
            { new SelectListItem{Text=SD.RoleAdmin,Value = SD.RoleAdmin},
              new SelectListItem{Text=SD.RoleCustomer,Value = SD.RoleCustomer}
            };
            ViewBag.RoleList = roleList;
            return View(registrationRequestDto);
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            _tokenProvider.ClearToken();
            return RedirectToAction("Index","Home");
        }

        //Sign User with .Net Identity
        private async Task SignInUser(LoginResponseDto model)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(model.Token);
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

            //Add Claims from jwt
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email, 
                jwt.Claims.FirstOrDefault(u=>u.Type== JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Name).Value));

            // add claim from claimtypes
            identity.AddClaim(new Claim(ClaimTypes.Name,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
            //Add claim Role
            identity.AddClaim(new Claim(ClaimTypes.Role,
               jwt.Claims.FirstOrDefault(u => u.Type == "role").Value));


            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
        public IActionResult AccessDenied(string? ReturnUrl)
        {
            return View();
        }
    }
}
