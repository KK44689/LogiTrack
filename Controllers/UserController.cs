using LogiTrack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // Endpoint to get user details by username
    [HttpGet("{username}")]
    public async Task<IActionResult> GetUserByUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username)) return BadRequest("Username cannot be empty.");

        var user = await _userManager.FindByNameAsync(username);
        if (user == null) return NotFound("User not found.");

        return Ok(user);
    }

    [HttpGet("role/{userId}")]
    public async Task<IActionResult> GetUserRole(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId)) return BadRequest("User ID cannot be empty.");

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound("User not found.");

        var roles = await _userManager.GetRolesAsync(user);
        return roles != null && roles.Any() ? Ok(roles) : NotFound("No roles assigned to this user.");
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userManager.Users.ToListAsync();
        if (!users.Any())
        {
            return NotFound("No users found.");
        }

        return Ok(users);
    }

    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteUserByID(string userId)
    {
        if (string.IsNullOrEmpty(userId)) return BadRequest("User ID cannot be empty.");

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound("User not found.");

        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded
            ? Ok($"User {user.UserName} deleted successfully.")
            : BadRequest("Failed to delete user.");
    }
}