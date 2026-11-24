using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;

namespace OrderDesk.Support;

/// <summary>
/// Exam convenience only: keeps session data in files under storage/sessions,
/// so the cart and the selected user survive restarting the app.
/// Entries do not expire; delete the folder to clear all sessions.
/// </summary>
public sealed class FileSessionCache : IDistributedCache
{
    private readonly string _directory;
    private readonly Lock _gate = new();

    public FileSessionCache(string directory)
    {
        _directory = Directory.CreateDirectory(directory).FullName;
    }

    public byte[]? Get(string key)
    {
        lock (_gate)
        {
            var path = PathFor(key);

            return File.Exists(path) ? File.ReadAllBytes(path) : null;
        }
    }

    public Task<byte[]?> GetAsync(string key, CancellationToken token = default) => Task.FromResult(Get(key));

    public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
    {
        lock (_gate)
        {
            File.WriteAllBytes(PathFor(key), value);
        }
    }

    public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
    {
        Set(key, value, options);

        return Task.CompletedTask;
    }

    public void Refresh(string key)
    {
    }

    public Task RefreshAsync(string key, CancellationToken token = default) => Task.CompletedTask;

    public void Remove(string key)
    {
        lock (_gate)
        {
            File.Delete(PathFor(key));
        }
    }

    public Task RemoveAsync(string key, CancellationToken token = default)
    {
        Remove(key);

        return Task.CompletedTask;
    }

    private string PathFor(string key) =>
        Path.Combine(_directory, Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(key))));
}
