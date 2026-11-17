using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Game.Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
namespace Game.REST.Controllers;
public sealed class RoleRequest
{
    [Required, EmailAddress] public string Email { get; set; } = "";
    [Required] public string Role { get; set; } = "";
}
[ApiController]
[Route("api/users")]
public sealed class UsersController(UserManager<GameUser> users) : ControllerBase
{
    [Authorize(Roles = "Reader,Editor,Admin")]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var user = await users.GetUserAsync(User);
        return user is null ? Unauthorized() : Ok(new { user.Email, Roles = await users.GetRolesAsync(user) });
    }
    [Authorize(Roles = "Admin")]
    [HttpPost("roles")]
    public async Task<IActionResult> AssignRole(RoleRequest request)
    {
        if (request.Role is not ("Reader" or "Editor" or "Admin")) return BadRequest("Невідома роль.");
        var user = await users.FindByEmailAsync(request.Email);
        if (user is null) return NotFound();
        if (await users.IsInRoleAsync(user, request.Role)) return NoContent();
        var result = await users.AddToRoleAsync(user, request.Role);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }
}
