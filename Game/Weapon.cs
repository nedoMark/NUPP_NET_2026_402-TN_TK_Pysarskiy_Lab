namespace Game;

public class Weapon
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
}