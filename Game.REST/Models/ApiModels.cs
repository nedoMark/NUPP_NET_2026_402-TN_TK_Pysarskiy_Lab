using System.ComponentModel.DataAnnotations;
namespace Game.REST.Models;
public sealed record CharacterResponse(Guid Id, string Name, int Health);
public sealed class CharacterRequest
{
    [Required, StringLength(100, MinimumLength = 1)] public string Name { get; set; } = "";
    [Range(0, 100000)] public int Health { get; set; }
}
public sealed record WeaponResponse(Guid Id, string Name, int Damage);
public sealed class WeaponRequest
{
    [Required, StringLength(100, MinimumLength = 1)] public string Name { get; set; } = "";
    [Range(0, 100000)] public int Damage { get; set; }
}
