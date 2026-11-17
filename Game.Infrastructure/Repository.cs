using Game.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
namespace Game.Infrastructure;
// Guid використовується замість int, оскільки ідентифікатори попередніх робіт — Guid.
public interface IRepository<T> where T : class, IEntityModel
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task<bool> Update(T entity);
    Task<bool> Delete(T entity);
    Task<IEnumerable<T>> GetPageAsync(int page, int amount);
    Task<bool> SaveAsync();
}
public sealed class Repository<T> : IRepository<T> where T : class, IEntityModel
{
    private readonly DbContextOptions<GameContext> options;
    public Repository(DbContextOptions<GameContext> options) => this.options = options;
    // Окремий DbContext для кожної операції: DbContext не є потокобезпечним.
    public async Task<T?> GetByIdAsync(Guid id) { await using var db = new GameContext(options); return await db.Set<T>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id); }
    public async Task<IEnumerable<T>> GetAllAsync() { await using var db = new GameContext(options); return await db.Set<T>().AsNoTracking().OrderBy(x => x.Id).ToListAsync(); }
    public async Task<IEnumerable<T>> GetPageAsync(int page, int amount) { await using var db = new GameContext(options); return await db.Set<T>().AsNoTracking().OrderBy(x => x.Id).Skip(checked((page - 1) * amount)).Take(amount).ToListAsync(); }
    public async Task AddAsync(T entity) { await using var db = new GameContext(options); db.Set<T>().Add(entity); await db.SaveChangesAsync(); }
    public async Task<bool> Update(T entity) { await using var db = new GameContext(options); if (!await db.Set<T>().AnyAsync(x => x.Id == entity.Id)) return false; db.Set<T>().Update(entity); await db.SaveChangesAsync(); return true; }
    public async Task<bool> Delete(T entity) { await using var db = new GameContext(options); var existing = await db.Set<T>().FindAsync(entity.Id); if (existing is null) return false; db.Remove(existing); await db.SaveChangesAsync(); return true; }
    public Task<bool> SaveAsync() => Task.FromResult(true); // Кожна операція вже збережена транзакційно.
}
