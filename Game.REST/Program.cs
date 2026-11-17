using Game;
using Game.Infrastructure;
using Game.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
var root = builder.Environment.ContentRootPath;
var defaultDb = File.Exists(Path.Combine(root, "game.db")) ? Path.Combine(root, "game.db") : Path.GetFullPath(Path.Combine(root, "..", "game.db"));
var dbPath = builder.Configuration["DatabasePath"] ?? defaultDb;
var options = new DbContextOptionsBuilder<GameContext>().UseSqlite($"Data Source={dbPath}").Options;
builder.Services.AddSingleton(options);
builder.Services.AddScoped<GameContext>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped(typeof(ICrudServiceAsync<>), typeof(DatabaseCrudService<>));
var app = builder.Build();
await using (var scope = app.Services.CreateAsyncScope())
    await scope.ServiceProvider.GetRequiredService<GameContext>().Database.MigrateAsync();
app.MapControllers();
app.Run();
public partial class Program { }

