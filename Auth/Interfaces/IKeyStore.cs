namespace Auth.Interfaces;

/// <summary>
/// Сервис доступа к ключам
/// </summary>
public interface IKeyStore
{
    /// <summary>
    /// Приватный ключ
    /// </summary>
    string PrivateKey { get; }
    /// <summary>
    /// Публичный ключ
    /// </summary>
    string PublicKey { get; }
}