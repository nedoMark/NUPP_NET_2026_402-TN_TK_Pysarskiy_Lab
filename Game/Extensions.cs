namespace Game;

// Метод розширення для Character
public static class CharacterExtensions
{
    public static string GetInfo(this Character character)
    {
        return $"{character.Name} — HP: {character.Health}";
    }
}