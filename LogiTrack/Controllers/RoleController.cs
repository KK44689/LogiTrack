using LogiTrack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogiTrack.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName)) return BadRequest("Role name cannot be empty.");

            var isRoleExists = await _roleManager.RoleExistsAsync(roleName);
            if (isRoleExists) return BadRequest("Role already exists.");

            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            return result.Succeeded ? Ok($"Role {roleName} created successfully.") : BadRequest($"Failed to create role {roleName}.");
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignRoleToUser(RoleModel model)
        {
            if (string.IsNullOrWhiteSpace(model.UserId) || string.IsNullOrWhiteSpace(model.RoleName))
                return BadRequest("User ID and role name cannot be empty.");

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return NotFound("User not found.");

            var isRoleExists = await _roleManager.RoleExistsAsync(model.RoleName);
            if (!isRoleExists) return BadRequest("Role does not exist.");

            var result = await _userManager.AddToRoleAsync(user, model.RoleName);
            return result.Succeeded ? Ok($"Role {model.RoleName} assigned to user {user.UserName} successfully.") : BadRequest($"Failed to assign role {model.RoleName} to user {user.UserName}.");
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return Ok(roles);
        }
    }
}