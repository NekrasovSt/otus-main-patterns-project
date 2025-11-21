using HistoryService.Interfaces;
using Path = System.IO.Path;

namespace HistoryService.Helpers;

/// <inheritdoc />
public class KeyStore : IKeyStore
{
    /// <summary>
    /// Конструктор
    /// </summary>
    public KeyStore(IConfiguration configuration)
    {
        var keyPath = configuration.GetValue<string>("publicKey") ??
                      throw new ArgumentNullException("configuration.publicKey");
        PublicKey = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), keyPath));
    }

    /// <inheritdoc />
    public string PublicKey { get; }
}