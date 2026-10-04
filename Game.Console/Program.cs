using Game;

// Створюємо CRUD-сервіс
CharacterService service = new CharacterService();

// Створюємо персонажів
Character hero = new Character("Hero", 100);
Character enemy = new Character("Goblin", 50);

// CREATE
service.Create(hero);
service.Create(enemy);

Console.WriteLine("=== CREATE ===");
Console.WriteLine($"Створено: {hero.Name}");
Console.WriteLine($"Створено: {enemy.Name}");

// READ
Console.WriteLine("\n=== READ ===");
Character found = service.Read(hero.Id);

if (found != null)
{
    Console.WriteLine($"Знайдено: {found.Name}, HP: {found.Health}");
}

// READ ALL
Console.WriteLine("\n=== READ ALL ===");

foreach (Character character in service.ReadAll())
{
    Console.WriteLine($"{character.Name}, HP: {character.Health}");
}

// UPDATE
Console.WriteLine("\n=== UPDATE ===");

hero.Health = 80;
service.Update(hero);

Console.WriteLine($"Після оновлення: {hero.Name}, HP: {hero.Health}");

// REMOVE
Console.WriteLine("\n=== REMOVE ===");

service.Remove(enemy);

Console.WriteLine($"Видалено: {enemy.Name}");

Console.WriteLine("\nЗалишилися персонажі:");

foreach (Character character in service.ReadAll())
{
    Console.WriteLine($"{character.Name}, HP: {character.Health}");
}

// Extension Method
Console.WriteLine("\n=== EXTENSION METHOD ===");
Console.WriteLine(hero.GetInfo());

// Подія Player
Console.WriteLine("\n=== EVENT ===");

Player player = new Player("Player", 100, "HeroPlayer");

player.LevelChanged += message =>
{
    Console.WriteLine(message);
};

player.AddExperience(100);

// Статичний метод
Console.WriteLine("\n=== STATIC METHOD ===");
Console.WriteLine(
    $"Кількість створених персонажів: {Character.GetCharacterCount()}"
);