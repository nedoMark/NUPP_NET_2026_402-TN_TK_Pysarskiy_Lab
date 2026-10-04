namespace Game;

// Делегат для повідомлення про зміну рівня
public delegate void LevelChangedHandler(string message);

public class Player : Character
{
    public int Level { get; set; }
    public int Experience { get; set; }
    public string Nickname { get; set; }

    // Подія
    public event LevelChangedHandler? LevelChanged;

    public Player(string name, int health, string nickname)
        : base(name, health)
    {
        Nickname = nickname;
        Level = 1;
        Experience = 0;
    }

    // Метод, який викликає подію
    public void AddExperience(int amount)
    {
        Experience += amount;

        if (Experience >= 100)
        {
            Level++;
            Experience -= 100;

            LevelChanged?.Invoke(
                $"Гравець {Nickname} досяг {Level} рівня!"
            );
        }
    }
}