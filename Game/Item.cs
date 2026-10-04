namespace Game;

public class Item
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
}