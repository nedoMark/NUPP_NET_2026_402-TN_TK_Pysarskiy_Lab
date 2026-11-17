namespace Game;

public class Enemy : Character
{
    public string EnemyType { get; set; }
    public int Damage { get; set; }
    public int Reward { get; set; }

    public Enemy(string name, int health, string enemyType, int damage, int reward)
        : base(name, health)
    {
        EnemyType = enemyType;
        Damage = damage;
        Reward = reward;
    }

    public new static Enemy CreateNew() => new($"Ворог-{Guid.NewGuid():N}", Random.Shared.Next(50, 201), "Монстр", Random.Shared.Next(1, 101), Random.Shared.Next(1, 501));
}
