namespace Game;

public class CharacterService : ICrudService<Character>
{
    private readonly List<Character> characters = new();

    // CREATE
    public void Create(Character element)
    {
        characters.Add(element);
    }

    // READ
    public Character Read(Guid id)
    {
        return characters.FirstOrDefault(c => c.Id == id)
            ?? throw new KeyNotFoundException("Персонажа не знайдено.");
    }

    // READ ALL
    public IEnumerable<Character> ReadAll()
    {
        return characters;
    }

    // UPDATE
    public void Update(Character element)
    {
        var existing = characters.FirstOrDefault(c => c.Id == element.Id);

        if (existing != null)
        {
            existing.Name = element.Name;
            existing.Health = element.Health;
        }
    }

    // REMOVE
    public void Remove(Character element)
    {
        characters.Remove(element);
    }
}