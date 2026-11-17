using Game;
using Game.Infrastructure;
using Game.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
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
builder.Services.AddIdentityApiEndpoints<GameUser>().AddRoles<IdentityRole>().AddEntityFrameworkStores<GameContext>();
builder.Services.AddAuthorization();
var app = builder.Build();
await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<GameContext>();
    await db.Database.MigrateAsync();
    var roles = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    foreach (var name in new[] { "Reader", "Editor", "Admin" })
        if (!await roles.RoleExistsAsync(name))
        {
            var created = await roles.CreateAsync(new IdentityRole(name));
            if (!created.Succeeded) throw new InvalidOperationException(string.Join(", ", created.Errors.Select(e => e.Description)));
        }
    // Початкового адміністратора можна створити лише при першому налаштуванні;
    // значення передаються через конфігурацію, а не зберігаються в коді.
    var users = scope.ServiceProvider.GetRequiredService<UserManager<GameUser>>();
    var email = builder.Configuration["BootstrapAdmin:Email"];
    var password = builder.Configuration["BootstrapAdmin:Password"];
    if (!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(password) && !(await users.GetUsersInRoleAsync("Admin")).Any())
    {
        if (await users.FindByEmailAsync(email) is not null) throw new InvalidOperationException("Початковий адміністратор має бути новим користувачем.");
        var admin = new GameUser { UserName = email, Email = email, EmailConfirmed = true };
        var created = await users.CreateAsync(admin, password);
        if (!created.Succeeded) throw new InvalidOperationException(string.Join(", ", created.Errors.Select(e => e.Description)));
        var assigned = await users.AddToRoleAsync(admin, "Admin");
        if (!assigned.Succeeded) throw new InvalidOperationException(string.Join(", ", assigned.Errors.Select(e => e.Description)));
    }
}
app.UseAuthentication();
app.UseAuthorization();
app.MapGroup("/identity").MapIdentityApi<GameUser>();
app.MapControllers();
app.Run();
public partial class Program { }
