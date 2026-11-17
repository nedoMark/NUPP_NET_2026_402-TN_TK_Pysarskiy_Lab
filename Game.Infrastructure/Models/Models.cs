namespace Game.Infrastructure.Models;
public interface IEntityModel { Guid Id { get; set; } }
public class CharacterModel : IEntityModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public int Health { get; set; }
    public List<ItemModel> Items { get; set; } = new();
    public static CharacterModel CreateNew() => new() { Name = $"Hero-{Guid.NewGuid():N}"[..13], Health = Random.Shared.Next(50, 201) };
}
public class PlayerModel : CharacterModel
{
    public int Level { get; set; }
    public int Experience { get; set; }
    public string Nickname { get; set; } = "";
    public PlayerProfileModel? Profile { get; set; }
}
public class EnemyModel : CharacterModel
{
    public string EnemyType { get; set; } = "";
    public int Damage { get; set; }
    public int Reward { get; set; }
}
public class PlayerProfileModel : IEntityModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PlayerId { get; set; }
    public PlayerModel Player { get; set; } = null!;
    public string Description { get; set; } = "";
}
public class ItemModel : IEntityModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public int Value { get; set; }
    public Guid CharacterId { get; set; }
    public CharacterModel Character { get; set; } = null!;
}
public class WeaponModel : IEntityModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public int Damage { get; set; }
    public List<TagModel> Tags { get; set; } = new();
}
public class TagModel : IEntityModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public List<WeaponModel> Weapons { get; set; } = new();
}
