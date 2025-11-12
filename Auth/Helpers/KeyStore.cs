using Auth.Interfaces;

namespace Auth.Helpers;

/// <inheritdoc />
public class KeyStore : IKeyStore
{
    /// <summary>
    /// Конструктор
    /// </summary>
    public KeyStore(IConfiguration configuration)
    {
        var keyPath = configuration.GetValue<string>("privateKey") ??
                      throw new ArgumentNullException("configuration.privateKey");
        PrivateKey = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), keyPath));
        keyPath = configuration.GetValue<string>("publicKey") ??
                  throw new ArgumentNullException("configuration.publicKey");
        PublicKey = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), keyPath));
    }

    /// <inheritdoc />
    public string PrivateKey { get; }

    /// <inheritdoc />
    public string PublicKey { get; }
}