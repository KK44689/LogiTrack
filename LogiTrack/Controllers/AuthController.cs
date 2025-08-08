using LogiTrack.Models;
using LogiTrack.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LogiTrack.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signinManager;
        private readonly JwtTokenServices _jwtTokenServices;

        public AuthController(UserManager<ApplicationUser> manager, SignInManager<ApplicationUser> signinManager, JwtTokenServices jwtTokenServices)
        {
            _userManager = manager;
            _signinManager = signinManager;
            _jwtTokenServices = jwtTokenServices;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded) return BadRequest(result.Errors);

            var roleName = string.IsNullOrEmpty(model.RoleName) ? "User" : model.RoleName;
            var token = _jwtTokenServices.GenerateToken(user, roleName);
            return Ok(new { token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return Unauthorized("Invalid credentials.");

            var result = await _signinManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (result.Succeeded)
            {
                var role = await _userManager.GetRolesAsync(user);
                var token = _jwtTokenServices.GenerateToken(user, role.FirstOrDefault() ?? "User");
                return Ok(new
                {
                    token = token
                });
            }

            return Unauthorized("Invalid credentials");
        }
    }
}