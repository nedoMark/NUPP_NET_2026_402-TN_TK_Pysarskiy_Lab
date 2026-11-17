using Microsoft.AspNetCore.Identity;
namespace Game.Infrastructure.Models;
public class GameUser : IdentityUser
{
    public string DisplayName { get; set; } = "";
}
