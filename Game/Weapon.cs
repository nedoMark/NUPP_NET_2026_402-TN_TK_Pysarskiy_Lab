namespace Game;

public class Weapon : IEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Damage { get; set; }

    public Weapon(string name, int damage)
    {
        Id = Guid.NewGuid();
        Name = name;
        Damage = damage;
    }

    public static Weapon CreateNew() => new($"Зброя-{Guid.NewGuid():N}", Random.Shared.Next(1, 101));
}
