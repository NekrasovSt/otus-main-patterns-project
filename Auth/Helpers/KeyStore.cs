using Auth.Interfaces;

namespace Auth.Helpers;

public class KeyStore: IKeyStore
{
    public KeyStore(IConfiguration configuration)
    {
        var keyPath = configuration.GetValue<string>("privateKey");
        PrivateKey = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), keyPath));
        keyPath = configuration.GetValue<string>("publicKey");
        PublicKey = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), keyPath));
    }
    public string PrivateKey { get; }
    public string PublicKey { get; }
}