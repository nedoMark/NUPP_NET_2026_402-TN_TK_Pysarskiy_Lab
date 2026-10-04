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
}