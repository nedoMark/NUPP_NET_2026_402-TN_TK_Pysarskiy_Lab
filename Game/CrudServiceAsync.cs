using System.Collections;
using System.Text.Json;

namespace Game;

// Колекція та її знімки захищені одним lock. Об'єкти, передані сервісу,
// не слід змінювати одночасно з читанням або збереженням.
public sealed class CrudServiceAsync<T> : ICrudServiceAsync<T> where T : class, IEntity
{
    private readonly Dictionary<Guid, T> items = new();
    private readonly object sync = new();
    private readonly SemaphoreSlim saveGate = new(1, 1);
    public string FilePath { get; }

    public CrudServiceAsync(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        FilePath = Path.GetFullPath(filePath);
    }

    public Task<bool> CreateAsync(T element)
    {
        ArgumentNullException.ThrowIfNull(element);
        lock (sync) return Task.FromResult(items.TryAdd(element.Id, element));
    }

    public Task<T> ReadAsync(Guid id)
    {
        lock (sync)
            return Task.FromResult(items.TryGetValue(id, out var value)
                ? value : throw new KeyNotFoundException($"Об'єкт {id} не знайдено."));
    }

    public Task<IEnumerable<T>> ReadAllAsync() => Task.FromResult<IEnumerable<T>>(Snapshot());

    // Нумерація сторінок починається з 1.
    public Task<IEnumerable<T>> ReadAllAsync(int page, int amount)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(page, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(amount, 1);
        long skip = (long)(page - 1) * amount;
        var snapshot = Snapshot();
        return Task.FromResult<IEnumerable<T>>(skip >= snapshot.Length
            ? Array.Empty<T>() : snapshot.Skip((int)skip).Take(amount).ToArray());
    }

    public Task<bool> UpdateAsync(T element)
    {
        ArgumentNullException.ThrowIfNull(element);
        lock (sync)
        {
            if (!items.ContainsKey(element.Id)) return Task.FromResult(false);
            items[element.Id] = element;
            return Task.FromResult(true);
        }
    }

    public Task<bool> RemoveAsync(T element)
    {
        ArgumentNullException.ThrowIfNull(element);
        lock (sync) return Task.FromResult(items.Remove(element.Id));
    }

    public async Task<bool> SaveAsync()
    {
        await saveGate.WaitAsync();
        try
        {
            T[] snapshot = Snapshot();
            string? directory = Path.GetDirectoryName(FilePath);
            if (directory is not null) Directory.CreateDirectory(directory);
            // Тимчасовий файл у тій самій папці не залишає частково записаного JSON.
            string temp = FilePath + "." + Guid.NewGuid() + ".tmp";
            try
            {
                await using (var stream = new FileStream(temp, FileMode.CreateNew, FileAccess.Write, FileShare.None, 4096, true))
                    await JsonSerializer.SerializeAsync(stream, snapshot, new JsonSerializerOptions { WriteIndented = true });
                File.Move(temp, FilePath, true);
                return true;
            }
            finally { if (File.Exists(temp)) File.Delete(temp); }
        }
        finally { saveGate.Release(); }
    }

    private T[] Snapshot() { lock (sync) return items.Values.ToArray(); }
    public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)Snapshot()).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
