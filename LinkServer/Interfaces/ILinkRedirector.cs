namespace LinkServer.Services;

/// <summary>
/// Сервис перенаправления
/// </summary>
public interface ILinkRedirector
{
    /// <summary>
    /// Получить ссылку для перенаправления
    /// </summary>
    Task<string> RedirectAsync(Dictionary<string, object> dictionary, CancellationToken token);
}