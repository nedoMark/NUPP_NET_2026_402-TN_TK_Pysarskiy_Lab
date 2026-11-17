namespace Game;

public class Item : IEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Value { get; set; }

    public Item(string name, int value)
    {
        Id = Guid.NewGuid();
        Name = name;
        Value = value;
    }

    public static Item CreateNew() => new($"Предмет-{Guid.NewGuid():N}", Random.Shared.Next(1, 1001));
}
