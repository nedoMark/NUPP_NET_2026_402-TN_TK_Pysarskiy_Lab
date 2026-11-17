using Game.Infrastructure.Models;
using Game;
using Microsoft.EntityFrameworkCore;
namespace Game.Infrastructure;
public sealed class DatabaseCrudService<T> : ICrudServiceAsync<T> where T : class, IEntityModel
{
    private readonly IRepository<T> repository;
    public DatabaseCrudService(IRepository<T> repository) => this.repository = repository;
    public async Task<bool> CreateAsync(T element) { if (await repository.GetByIdAsync(element.Id) is not null) return false; try { await repository.AddAsync(element); return true; } catch (DbUpdateException) { if (await repository.GetByIdAsync(element.Id) is not null) return false; throw; } }
    public async Task<T> ReadAsync(Guid id) => await repository.GetByIdAsync(id) ?? throw new KeyNotFoundException();
    public Task<IEnumerable<T>> ReadAllAsync() => repository.GetAllAsync();
    public Task<IEnumerable<T>> ReadAllAsync(int page, int amount) { ArgumentOutOfRangeException.ThrowIfLessThan(page, 1); ArgumentOutOfRangeException.ThrowIfLessThan(amount, 1); return repository.GetPageAsync(page, amount); }
    public Task<bool> UpdateAsync(T element) => repository.Update(element);
    public Task<bool> RemoveAsync(T element) => repository.Delete(element);
    public Task<bool> SaveAsync() => repository.SaveAsync();
    public IEnumerator<T> GetEnumerator() => ReadAllAsync().GetAwaiter().GetResult().GetEnumerator();
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
}

