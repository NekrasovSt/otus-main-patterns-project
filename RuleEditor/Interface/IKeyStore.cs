namespace RuleEditor.Interface;

/// <summary>
/// Доступ к ключам
/// </summary>
public interface IKeyStore
{
    /// <summary>
    /// Публичный ключь
    /// </summary>
    string PublicKey { get; }
}