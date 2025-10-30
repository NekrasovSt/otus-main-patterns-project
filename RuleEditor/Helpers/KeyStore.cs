using RuleEditor.Interface;

namespace RuleEditor.Helpers;

public class KeyStore: IKeyStore
{
    public KeyStore(IConfiguration configuration)
    {
        var keyPath = configuration.GetValue<string>("publicKey");
        PublicKey = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), keyPath));
    }
    public string PublicKey { get; }
}