namespace Game;

public class Character
{
    // Властивості класу
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Health { get; set; }

    // Статичне поле
    private static int characterCount;

    // Статичний конструктор
    static Character()
    {
        characterCount = 0;
    }

    // Звичайний конструктор
    public Character(string name, int health)
    {
        Id = Guid.NewGuid();
        Name = name;
        Health = health;
        characterCount++;
    }

    // Метод
    public void TakeDamage(int damage)
    {
        Health -= damage;

        if (Health < 0)
        {
            Health = 0;
        }
    }

    // Статичний метод
    public static int GetCharacterCount()
    {
        return characterCount;
    }
}