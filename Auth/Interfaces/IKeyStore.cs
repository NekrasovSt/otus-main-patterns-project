namespace Auth.Interfaces;

public interface IKeyStore
{
    string PrivateKey { get; }
    string PublicKey { get; }
}