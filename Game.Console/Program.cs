using System.Text;
using Game.Infrastructure;
using Game.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

Console.OutputEncoding = Encoding.UTF8;
string path = Path.GetFullPath(args.Length > 0 ? args[0] : "game.db");
var options = new DbContextOptionsBuilder<GameContext>().UseSqlite($"Data Source={path}").Options;
await using (var db = new GameContext(options))
{
    await db.Database.MigrateAsync();
    if (!await db.Players.AnyAsync())
    {
        var player = new PlayerModel { Name = "Artem", Health = 150, Nickname = "Hero", Level = 5, Experience = 35, Profile = new PlayerProfileModel { Description = "Досвідчений гравець" } };
        player.Items.Add(new ItemModel { Name = "Зілля", Value = 50 });
        player.Items.Add(new ItemModel { Name = "Ключ", Value = 20 });
        db.Players.Add(player);
        db.Enemies.Add(new EnemyModel { Name = "Goblin", Health = 80, EnemyType = "Monster", Damage = 25, Reward = 100 });
        var tag = new TagModel { Name = "Рідкісна" };
        db.Weapons.Add(new WeaponModel { Name = "Меч", Damage = 40, Tags = { tag } });
        db.Weapons.Add(new WeaponModel { Name = "Лук", Damage = 30, Tags = { tag } });
        await db.SaveChangesAsync();
    }
}
var service = new DatabaseCrudService<CharacterModel>(new Repository<CharacterModel>(options));
var character = CharacterModel.CreateNew();
Console.WriteLine("Лабораторна робота №3: Entity Framework Core та SQLite");
Console.WriteLine($"База даних: {path}");
Console.WriteLine($"Створено: {await service.CreateAsync(character)}; ID: {character.Id}");
var loaded = await service.ReadAsync(character.Id);
Console.WriteLine($"Знайдено: {loaded.Name}, здоров'я: {loaded.Health}");
loaded.Health = 175;
Console.WriteLine($"Оновлено: {await service.UpdateAsync(loaded)}");
Console.WriteLine($"Перевірка оновлення: {(await service.ReadAsync(loaded.Id)).Health}");
Console.WriteLine($"Усього персонажів: {(await service.ReadAllAsync()).Count()}; перша сторінка (2): {(await service.ReadAllAsync(1, 2)).Count()}");
Console.WriteLine($"Видалено тестового персонажа: {await service.RemoveAsync(loaded)}");
Console.WriteLine($"Збережено: {await service.SaveAsync()}");
await using var check = new GameContext(options);
Console.WriteLine($"Гравці: {await check.Players.CountAsync()}, вороги: {await check.Enemies.CountAsync()}, предмети: {await check.Items.CountAsync()}, профілі: {await check.Profiles.CountAsync()}, зброя: {await check.Weapons.CountAsync()}, теги: {await check.Tags.CountAsync()}");
